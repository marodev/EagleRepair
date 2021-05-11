using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EagleRepair.Ast.Parser;
using EagleRepair.Ast.Rewriter;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;

namespace EagleRepair.Ast
{
    public class Engine : IEngine
    {
        private readonly IChangeTracker _changeTracker;
        private readonly IProgressBar _progressBar;
        private readonly ISolutionParser _solutionParser;
        private readonly ICollection<AbstractRewriter> _visitors;

        public Engine(ICollection<AbstractRewriter> visitors, ISolutionParser solutionParser, IProgressBar progressBar,
            IChangeTracker changeTracker)
        {
            _visitors = visitors;
            _solutionParser = solutionParser;
            _progressBar = progressBar;
            _changeTracker = changeTracker;
        }

        public async Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules)
        {
            // report progress to console
            _progressBar.Report(0.0, "Opening solution " + solutionFilePath + " ...");
            var solution = await _solutionParser.OpenSolutionAsync(solutionFilePath);
            // select all files
            var files = solution.Projects.SelectMany(p => p.Documents).ToList();
            // filter rules to apply
            var visitors = FilterVisitors(_visitors, rules.ToList());
            // rewrite the syntax tree
            var newSolution = await VisitNodes(solution, files, visitors);
            // apply the changes (if any) to the solution
            return ReferenceEquals(newSolution, _solutionParser.Workspace().CurrentSolution) ||
                   _solutionParser.Workspace().TryApplyChanges(newSolution);
        }

        private static IList<AbstractRewriter> FilterVisitors(IEnumerable<AbstractRewriter> visitors, IList<Rule> rules)
        {
            return (from visitor in visitors
                    let className = visitor.GetType().Name
                    where rules.Any(r => className.EndsWith(r.ToString()))
                    select visitor)
                .ToList();
        }

        private async Task<Solution> VisitNodes(Solution solution, ICollection<Document> documents,
            IList<AbstractRewriter> visitors)
        {
            var totalDocuments = documents.Count;
            var counter = 1;
            foreach (var document in documents)
            {
                // report progress to console
                _progressBar.Report((double)counter / totalDocuments, document.Name);
                foreach (var rewriter in visitors)
                {
                    // Fetch document in solution
                    var modifiedDoc = solution.GetDocument(document.Id);
                    if (modifiedDoc is null)
                    {
                        continue;
                    }

                    // Selects the syntax tree
                    var syntaxTree = await modifiedDoc.GetSyntaxTreeAsync();
                    if (syntaxTree is null)
                    {
                        Console.WriteLine($"Error: Unable to parse SyntaxTree for document: {modifiedDoc.Name}");
                        continue;
                    }

                    var root = await syntaxTree.GetRootAsync();
                    var semanticModel = await modifiedDoc.GetSemanticModelAsync();

                    rewriter.SemanticModel = semanticModel;
                    rewriter.Workspace = solution.Workspace;
                    rewriter.FilePath = modifiedDoc.FilePath;
                    rewriter.ProjectName = modifiedDoc.Project.Name;

                    SyntaxNode newRoot;
                    try
                    {
                        newRoot = rewriter.Visit(root);
                    }
                    catch (Exception)
                    {
                        // TODO: we might log the exception at a later point and offer a verbose mode
                        continue;
                    }

                    if (root.IsEquivalentTo(newRoot))
                    {
                        // no changes
                        continue;
                    }

                    var diagnosticsForDocBeforeChanges = semanticModel?.GetDiagnostics();

                    // Exchanges the document in the solution by the newly generated document
                    solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);

                    // Note: This operation is very expensive
                    var diagnosticsForDocAfterChanges = await GetDiagnostics(solution, document.Id);

                    if (diagnosticsForDocBeforeChanges is null ||
                        diagnosticsForDocBeforeChanges.Value.Length < diagnosticsForDocAfterChanges.Length)
                    {
                        // something went wrong, revert changes!
                        solution = solution.WithDocumentSyntaxRoot(document.Id, root);
                        _changeTracker.Revert();
                    }
                    else
                    {
                        // confirm reported changes that will be displayed to the user
                        _changeTracker.Confirm();
                    }
                }

                counter++;
            }

            _progressBar.Report(100.0, "Done.");

            return solution;
        }

        private static async Task<ImmutableArray<Diagnostic>> GetDiagnostics(Solution solution, DocumentId documentId)
        {
            var foundDocument = solution.GetDocument(documentId);

            if (foundDocument == null)
            {
                return new ImmutableArray<Diagnostic>();
            }

            var semanticModel = await foundDocument.GetSemanticModelAsync();
            return semanticModel?.GetDiagnostics() ?? new ImmutableArray<Diagnostic>();
        }
    }
}
