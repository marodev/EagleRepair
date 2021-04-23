using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ast.Parser;
using Ast.SyntaxRewriter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Ast
{
    public class Engine
    {
        public static async Task<bool> RunAsync(string solutionFilePath, IEnumerable<Rule> rules)
        {
            using var solutionParser = new SolutionParser();
            var solution = await solutionParser.OpenSolutionAsync(solutionFilePath);
            // select all files
            var files = solution.Projects.SelectMany(p => p.Documents).ToList();
            var syntaxRewriters = FetchRewriters(rules);
            // rewrite the syntax tree
            solution = await VisitNodes(solution, files, syntaxRewriters);
            // apply the changes to the solution
            var result = solutionParser.Workspace.TryApplyChanges(solution);

            return result;
        }

        private static async Task<Solution> VisitNodes(Solution solution, ICollection<Document> documents,
            ICollection<CSharpSyntaxRewriter> rewriters)
        {
            foreach (var rewriter in rewriters)
            {
                foreach (var document in documents)
                {
                    // Selects the syntax tree
                    var syntaxTree = await document.GetSyntaxTreeAsync();
                    if (syntaxTree is null)
                    {
                        Console.WriteLine($"Unable to parse SyntaxTree for document: {document.Name}");
                        continue;
                    }

                    var root = await ModifySyntaxTree(document, rewriter);

                    // Exchanges the document in the solution by the newly generated document
                    solution = solution.WithDocumentSyntaxRoot(document.Id, root);
                }
            }

            return solution;
        }

        public static async Task<SyntaxNode> ModifySyntaxTree(Document document, CSharpSyntaxRewriter rewriter)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync();
            return await ModifySyntaxTree(syntaxTree, rewriter);
        }

        public static async Task<string> ModifySyntaxTree(string sourceCode, CSharpSyntaxRewriter rewriter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var node = await ModifySyntaxTree(syntaxTree, rewriter);
            return node.ToFullString();
        }

        private static async Task<SyntaxNode> ModifySyntaxTree(SyntaxTree syntaxTree, CSharpSyntaxRewriter rewriter)
        {
            var root = await syntaxTree.GetRootAsync();
            return rewriter.Visit(root);
        }

        private static ICollection<CSharpSyntaxRewriter> FetchRewriters(IEnumerable<Rule> rules)
        {
            
            List<CSharpSyntaxRewriter> syntaxRewriters = new();
            foreach (var rule in rules)
            {
                switch (rule)
                {
                    case Rule.Dummy:
                        syntaxRewriters.Add(new DummySyntaxRewriter());
                        break;
                    case Rule.Foo:
                        break;
                    case Rule.UseMethodAny:
                        syntaxRewriters.Add(new UseMethodAnySyntaxRewriter());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rule));
                }
            }

            return syntaxRewriters;
        }
    }
}