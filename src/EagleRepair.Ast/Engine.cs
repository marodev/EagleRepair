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
        private readonly ICollection<AbstractRewriter> _commands;
        private readonly IProgressBar _progressBar;
        private readonly ISolutionParser _solutionParser;

        public Engine(ICollection<AbstractRewriter> commands, ISolutionParser solutionParser, IProgressBar progressBar)
        {
            _commands = commands;
            _solutionParser = solutionParser;
            _progressBar = progressBar;
        }

        public async Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules)
        {
            // report progress to console
            _progressBar.Report(0.0, "Opening solution " + solutionFilePath + " ...");
            var solution = await _solutionParser.OpenSolutionAsync(solutionFilePath);
            // select all files
            var files = solution.Projects.SelectMany(p => p.Documents).ToList();
            // rewrite the syntax tree
            var newSolution = await VisitNodes(solution, files);
            // apply the changes (if any) to the solution
            return ReferenceEquals(newSolution, _solutionParser.Workspace().CurrentSolution) ||
                   _solutionParser.Workspace().TryApplyChanges(newSolution);
        }

        private async Task<Solution> VisitNodes(Solution solution, ICollection<Document> documents)
        {
            var totalDocuments = documents.Count;
            var counter = 1;
            foreach (var document in documents)
            {
                // report progress to console
                _progressBar.Report((double)counter / totalDocuments, document.Name);
                foreach (var rewriter in _commands)
                {
                    // Selects the syntax tree
                    var syntaxTree = await document.GetSyntaxTreeAsync();
                    if (syntaxTree is null)
                    {
                        Console.WriteLine($"Error: Unable to parse SyntaxTree for document: {document.Name}");
                        continue;
                    }

                    var root = await syntaxTree.GetRootAsync();
                    var semanticModel = await document.GetSemanticModelAsync();

                    rewriter.SemanticModel = semanticModel;
                    rewriter.Workspace = solution.Workspace;
                    rewriter.FilePath = document.FilePath;
                    rewriter.ProjectName = document.Project.Name;

                    var newRoot = rewriter.Visit(root);

                    if (root.IsEquivalentTo(newRoot))
                    {
                        continue;
                    }

                    var diagnosticsForDocBeforeChanges = await GetDiagnostics(solution, document);

                    // Exchanges the document in the solution by the newly generated document
                    solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);

                    var diagnosticsForDocAfterChanges = await GetDiagnostics(solution, document);

                    if (diagnosticsForDocBeforeChanges.Length < diagnosticsForDocAfterChanges.Length)
                    {
                        // something went wrong, revert changes!
                        solution = solution.WithDocumentSyntaxRoot(document.Id, root);
                    }
                }

                counter++;
            }

            _progressBar.Report(100.0, "Done.");

            return solution;
        }

        private static async Task<ImmutableArray<Diagnostic>> GetDiagnostics(Solution solution, TextDocument document)
        {
            var documents = solution.GetProject(document.Project.Id)?.Documents;

            var foundDocument = documents?.FirstOrDefault(d => d.Id == document.Id);

            if (foundDocument == null)
            {
                return new ImmutableArray<Diagnostic>();
            }

            var model = await foundDocument.GetSemanticModelAsync();

            return model?.Compilation.GetDiagnostics() ?? new ImmutableArray<Diagnostic>();
        }
    }
}
