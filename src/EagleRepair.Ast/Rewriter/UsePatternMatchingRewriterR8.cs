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
    public class UsePatternMatchingRewriterR8 : AbstractRewriter
    {
        public UsePatternMatchingRewriterR8(IChangeTracker changeTracker, ITypeService typeService,
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
            var childOfIfCondition = new SyntaxAnnotation("ChildOfIfCondition");
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

                    var newConditionExpr = RewriteService
                        .CreateIsPattern(binaryExpr.Left, s, identifierName, binaryExprToReplace);

                    if (!oldNewNodeDict.ContainsKey(localDeclaration))
                    {
                        oldNewNodeDict.Add(localDeclaration, null); // null -> remove node
                    }

                    if (oldNewNodeDict.ContainsKey(binaryExprToReplace))
                    {
                        continue;
                    }

                    if (binaryExprToReplace.Parent is IfStatementSyntax)
                    {
                        newConditionExpr = newConditionExpr.WithAdditionalAnnotations(childOfIfCondition);
                    }

                    oldNewNodeDict.Add(binaryExprToReplace, newConditionExpr);
                }
            }

            if (!oldNewNodeDict.Any())
            {
                // there are no changes
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
                    RuleId = nameof(Rule.R8),
                    LineNr = lineNumber,
                    FilePath = FilePath,
                    ProjectName = ProjectName,
                    Text = message
                });
            }

            var ifStatementChildren = newMethod.GetAnnotatedNodes(childOfIfCondition);
            var ifStatementsToReplace = AddLeadingLineFeedToIfStatements(ifStatementChildren);

            newMethod = newMethod.ReplaceNodes(ifStatementsToReplace.Keys.AsEnumerable(),
                (n1, n2) => ifStatementsToReplace[n1]);

            return base.VisitMethodDeclaration(newMethod);
        }

        private static IDictionary<SyntaxNode, SyntaxNode> AddLeadingLineFeedToIfStatements(
            IEnumerable<SyntaxNode> ifStatementChildren)
        {
            var ifStatementsToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var ifStatementChild in ifStatementChildren)
            {
                var firstChild = ifStatementChild.Parent?.Parent?.DescendantNodes().FirstOrDefault();

                if (firstChild == null)
                {
                    continue;
                }

                if (firstChild.IsEquivalentTo(ifStatementChild.Parent))
                {
                    continue;
                }

                // node is not first child, we want a new line just before the if condition
                // }            --> some other block
                //              --> target new line
                // if ( ... )   --> our if condition
                var existingTrivia = ifStatementChild.Parent.GetLeadingTrivia().Insert(0, SyntaxFactory.LineFeed);
                ifStatementsToReplace.Add(ifStatementChild.Parent,
                    ifStatementChild.Parent.WithLeadingTrivia(existingTrivia));
            }

            return ifStatementsToReplace;
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