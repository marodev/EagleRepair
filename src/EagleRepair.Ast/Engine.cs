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
        private readonly IFaultTracker _faultTracker;
        private readonly IProgressBar _progressBar;
        private readonly ISolutionParser _solutionParser;
        private readonly ICollection<AbstractRewriter> _visitors;

        public Engine(ICollection<AbstractRewriter> visitors, ISolutionParser solutionParser, IProgressBar progressBar,
            IChangeTracker changeTracker, IFaultTracker faultTracker)
        {
            _visitors = visitors;
            _solutionParser = solutionParser;
            _progressBar = progressBar;
            _changeTracker = changeTracker;
            _faultTracker = faultTracker;
        }

        public async Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules)
        {
            // filter rules to apply
            var visitors = FilterVisitors(_visitors, rules.ToList());
            Console.WriteLine("Found the following rules to apply: " +
                              $"{string.Join(", ", visitors.Select(v => v.GetType().Name))}" +
                              Environment.NewLine);
            // report progress to console
            _progressBar.Report(0.0, "Opening solution " + solutionFilePath + " ...");
            var solution = await _solutionParser.OpenSolutionAsync(solutionFilePath);
            // select all files
            var files = FilterCSharpFiles(solution);
            // Console.WriteLine($"Found {files.Count} C# files.");
            // rewrite the syntax tree
            var newSolution = await VisitNodes(solution, files, visitors);
            // apply the changes (if any) to the solution
            return ReferenceEquals(newSolution, _solutionParser.Workspace().CurrentSolution) ||
                   _solutionParser.Workspace().TryApplyChanges(newSolution);
        }

        private static IList<Document> FilterCSharpFiles(Solution solution)
        {
            // TODO: Currently, we consider the assemblies of one target framework
            // e.g, if the project was compiled against .NET 5 and .NET Core 3, we process all documents from .NET 5
            return solution.Projects.GroupBy(p => p.FilePath)
                .Select(g => g.OrderBy(pro => pro.Name).FirstOrDefault())
                .SelectMany(p => p?.Documents).ToList();
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
                foreach (var visitor in visitors)
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
                        _faultTracker.Add(visitor.GetType().Name, document.FilePath,
                            $"Error: Unable to parse SyntaxTree for document: {modifiedDoc.Name}", "-", "-");
                        continue;
                    }

                    var root = await syntaxTree.GetRootAsync();
                    var semanticModel = await modifiedDoc.GetSemanticModelAsync();

                    visitor.SemanticModel = semanticModel;
                    visitor.Workspace = solution.Workspace;
                    visitor.FilePath = modifiedDoc.FilePath;
                    visitor.ProjectName = modifiedDoc.Project.Name;

                    SyntaxNode newRoot;
                    try
                    {
                        newRoot = visitor.Visit(root);
                    }
                    catch (Exception e)
                    {
                        _faultTracker.Add(visitor.GetType().Name, document.FilePath,
                            $"Error: Unable to create valid SyntaxTree for document: {modifiedDoc.Name}. " +
                            $"Caught exception: {e}, Message: {e.Message}",
                            root.ToString(), "-");
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
                        diagnosticsForDocBeforeChanges.Value.Length < diagnosticsForDocAfterChanges.Length &&
                        !IsDiffNotNeededUsingDirectiveError(diagnosticsForDocBeforeChanges.Value,
                            diagnosticsForDocAfterChanges))
                    {
                        // something went wrong, revert changes!
                        solution = solution.WithDocumentSyntaxRoot(document.Id, root);
                        if (diagnosticsForDocBeforeChanges is not null)
                        {
                            _faultTracker.Add(visitor.GetType().Name, document.FilePath,
                                "Error: The created Syntax Tree is semantically incorrect.",
                                root.ToString(), newRoot.ToString(),
                                string.Join(",", diagnosticsForDocBeforeChanges.Value),
                                string.Join(",", diagnosticsForDocAfterChanges));
                        }
                        else
                        {
                            _faultTracker.Add(visitor.GetType().Name, document.FilePath,
                                "Error: Failed to create a semantic model.",
                                root.ToString(), newRoot.ToString());
                        }

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

        private static bool IsDiffNotNeededUsingDirectiveError(ImmutableArray<Diagnostic> before,
            ImmutableArray<Diagnostic> after)
        {
            const string NotNeededUsingDirectiveCode = "CS8019";
            return before.Length == after.Length - 1 &&
                   before.Count(d => d.Id.ToString().Equals(NotNeededUsingDirectiveCode)) ==
                   after.Count(d => d.Id.ToString().Equals(NotNeededUsingDirectiveCode)) - 1;
        }
    }
}
