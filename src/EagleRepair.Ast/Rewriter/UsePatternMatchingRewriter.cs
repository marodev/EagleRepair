using System.Collections.Generic;
using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace EagleRepair.Ast.Rewriter
{
    public class UsePatternMatchingRewriter : AbstractRewriter
    {
        public UsePatternMatchingRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var localDeclarationStatements = node.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().ToList();
            var binaryExprStmts = node.DescendantNodes().OfType<BinaryExpressionSyntax>().ToList();

            if (!localDeclarationStatements.Any() || !binaryExprStmts.Any())
            {
                return base.VisitMethodDeclaration(node);
            }

            var oldNewNodeDict = new Dictionary<CSharpSyntaxNode, CSharpSyntaxNode>();
            foreach (var localDeclaration in localDeclarationStatements)
            {
                var declaration = localDeclaration.Declaration.Variables.FirstOrDefault();

                if (declaration?.Initializer is null)
                {
                    continue;
                }

                var identifierName = declaration.Identifier.ToString();
                var expression = declaration.Initializer.Value;

                if (expression is not BinaryExpressionSyntax binaryExpr)
                {
                    continue;
                }

                var left = binaryExpr.Left.ToString();
                var right = binaryExpr.Right.ToString();
                var op = binaryExpr.OperatorToken.ToString();

                if (!"as".Equals(op))
                {
                    continue;
                }

                var declaredSymbol = ModelExtensions.GetDeclaredSymbol(SemanticModel, declaration);
                var symbolReferences =
                    SymbolFinder.FindReferencesAsync(declaredSymbol, Workspace.CurrentSolution).Result;
                var localReferences = symbolReferences.First().Locations;

                var count = localReferences.Count();
                if (count > 2)
                {
                    // we can't refactor it with a high probability
                    continue;
                }

                var binaryExpressionsToReplace = FindBinaryExprToReplace(identifierName, binaryExprStmts);

                if (!binaryExpressionsToReplace.Any())
                {
                    continue;
                }

                foreach (var binaryExprToReplace in binaryExpressionsToReplace)
                {
                    TypeSyntax s;
                    if (binaryExpr.Right is PredefinedTypeSyntax predefinedTypeSyntax)
                    {
                        s = predefinedTypeSyntax;
                    }
                    else
                    {
                        s = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(right));
                    }

                    var newConditionExpr = RewriteService.CreateIsPattern(binaryExpr.Left, s, identifierName);

                    if (!oldNewNodeDict.ContainsKey(localDeclaration))
                    {
                        oldNewNodeDict.Add(localDeclaration, null); // null -> remove node
                    }

                    if (!oldNewNodeDict.ContainsKey(binaryExprToReplace))
                    {
                        oldNewNodeDict.Add(binaryExprToReplace, newConditionExpr);
                    }
                }
            }

            if (!oldNewNodeDict.Any())
            {
                return base.VisitMethodDeclaration(node);
            }

            var newMethod = node.ReplaceNodes(oldNewNodeDict.Keys.AsEnumerable(),
                (n1, n2) => oldNewNodeDict[n1]);

            foreach (var nodeToUpdate in oldNewNodeDict)
            {
                var lineNumber = $"{DisplayService.GetLineNumber(nodeToUpdate.Key)}";
                var message = ReSharper.UsePatternMatchingMessage;
                ChangeTracker.Stage(new Message
                {
                    Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message
                });
            }

            return base.VisitMethodDeclaration(newMethod);
        }

        private static List<BinaryExpressionSyntax> FindBinaryExprToReplace(string variableName,
            IEnumerable<BinaryExpressionSyntax> binaryExprStmts)
        {
            var found = new List<BinaryExpressionSyntax>();

            foreach (var binaryExpr in binaryExprStmts)
            {
                var left = binaryExpr.Left.ToString();
                var op = binaryExpr.OperatorToken.ValueText;
                var right = binaryExpr.Right.ToString();

                if (!"!=".Equals(op) || !left.Equals(variableName) || !right.Equals("null"))
                {
                    continue;
                }

                found.Add(binaryExpr);
            }

            return found;
        }
    }
}
