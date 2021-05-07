using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EagleRepair.Ast.Parser;
using EagleRepair.Ast.Rewriter;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast
{
    public class Engine : IEngine
    {
        private readonly IChangeTracker _changeTracker;
        private readonly ICollection<AbstractRewriter> _commands;
        private readonly ISolutionParser _solutionParser;

        public Engine(ICollection<AbstractRewriter> commands, IChangeTracker changeTracker,
            ISolutionParser solutionParser)
        {
            _commands = commands;
            _changeTracker = changeTracker;
            _solutionParser = solutionParser;
        }

        public async Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules)
        {
            var solution = await _solutionParser.OpenSolutionAsync(solutionFilePath);
            // select all files
            var files = solution.Projects.SelectMany(p => p.Documents).ToList();
            // rewrite the syntax tree
            var newSolution = await VisitNodes(solution, files);
            if (ReferenceEquals(newSolution, _solutionParser.Workspace().CurrentSolution))
            {
                Console.WriteLine("ReferenceEquals(newSolution, solutionParser.Workspace.CurrentSolution)");
                return true;
            }

            Console.WriteLine("solutionParser.Workspace.TryApplyChanges(solution)");
            // apply the changes to the solution
            return _solutionParser.Workspace().TryApplyChanges(newSolution);
        }

        private async Task<Solution> VisitNodes(Solution solution, ICollection<Document> documents)
        {
            foreach (var document in documents)
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

                // Exchanges the document in the solution by the newly generated document
                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }

            return solution;
        }

        public static async Task<string> ModifySyntaxTree(string sourceCode, CSharpSyntaxRewriter rewriter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = await syntaxTree.GetRootAsync();
            var newRoot = rewriter.Visit(root);
            return newRoot.ToFullString();
        }
    }
}
