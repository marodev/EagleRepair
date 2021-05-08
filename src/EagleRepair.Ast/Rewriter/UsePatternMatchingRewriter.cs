using System.Collections.Generic;
using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
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
            var ifStatements = node.DescendantNodes().OfType<IfStatementSyntax>().ToList();

            if (!localDeclarationStatements.Any() || !ifStatements.Any())
            {
                return base.VisitMethodDeclaration(node);
            }

            var oldNewNodeDict = new Dictionary<StatementSyntax, StatementSyntax>();
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
                
                var declaredSymbol = SemanticModel.GetDeclaredSymbol(declaration);
                var symbolReferences = SymbolFinder.FindReferencesAsync(declaredSymbol, Workspace.CurrentSolution).Result;
                var localReferences = symbolReferences.First().Locations;

                var count = localReferences.Count();
                if (count > 2)
                {
                    // we can't refactor it with a high probability
                    continue;
                }

                var ifStatementsToReplace = FindIfStatements(identifierName, ifStatements);

                if (!ifStatementsToReplace.Any())
                {
                    continue;
                }

                foreach (var ifStatementToReplace in ifStatementsToReplace)
                {
                    var newConditionExpr = RewriteService.CreateIsPattern(left, right, identifierName);
                    var newIfStatementNode = ifStatementToReplace.WithCondition(newConditionExpr);

                    if (!oldNewNodeDict.ContainsKey(localDeclaration))
                    {
                        oldNewNodeDict.Add(localDeclaration, null); // null -> remove node
                    }

                    if (!oldNewNodeDict.ContainsKey(ifStatementToReplace))
                    {
                        oldNewNodeDict.Add(ifStatementToReplace, newIfStatementNode);
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
                ChangeTracker.Add(new Message
                {
                    Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message
                });
            }

            return base.VisitMethodDeclaration(newMethod);
        }

        private static List<IfStatementSyntax> FindIfStatements(string variableName,
            IEnumerable<IfStatementSyntax> ifStatements)
        {
            var found = new List<IfStatementSyntax>();

            foreach (var ifStatement in ifStatements)
            {
                if (ifStatement.Condition is not BinaryExpressionSyntax binaryExpr)
                {
                    continue;
                }

                var left = binaryExpr.Left.ToString();
                var op = binaryExpr.OperatorToken.ValueText;
                var right = binaryExpr.Right.ToString();

                if (!"!=".Equals(op) || !left.Equals(variableName) || !right.Equals("null"))
                {
                    continue;
                }

                found.Add(ifStatement);
            }

            return found;
        }
    }
}
