using System.Linq;
using EagleRepair.Ast.ReservedToken;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EagleRepair.Ast.Rewriter
{
    public class UseMethodAnyRewriter : AbstractRewriter
    {
        private bool _usesLinqDirective;

        public UseMethodAnyRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) :
            base(changeTracker, typeService, rewriteService, displayService)
        {
        }


        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var resultNode = base.VisitCompilationUnit(node);

            if (!_usesLinqDirective)
            {
                return resultNode;
            }

            // reset state
            _usesLinqDirective = false;

            return RewriteService.InjectUsingDirective((CompilationUnitSyntax)resultNode, "System.Linq");
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            // we are looking for pattern such as list.Count > 0
            var countNode = node.DescendantNodes().OfType<IdentifierNameSyntax>()
                .FirstOrDefault(n => n.Identifier.ValueText.Equals("Count"));

            if (countNode is null)
                // .Count() or .Count does not exist
            {
                return base.VisitBinaryExpression(node);
            }

            if (countNode.Parent is null)
            {
                return base.VisitBinaryExpression(node);
            }

            var typeSymbol = SemanticModel.GetTypeInfo(countNode.Parent.ChildNodes().ElementAt(0))
                .Type;

            if (typeSymbol == null)
            {
                return base.VisitBinaryExpression(node);
            }

            var containingNamespace = typeSymbol
                .ContainingNamespace
                ?.ToDisplayString();

            if (string.IsNullOrEmpty(containingNamespace))
            {
                return base.VisitBinaryExpression(node);
            }

            if (!TypeService.InheritsFromIEnumerable(containingNamespace))
            {
                return base.VisitBinaryExpression(node);
            }

            if (node.Left is ConditionalAccessExpressionSyntax)
            {
                // n?.PeriodicBackups.Count > 0 --> can't be transformed to n?.PeriodicBackups.Any() (System.Nullable<boolean>)
                return base.VisitBinaryExpression(node);
            }

            var newNode = ReplaceCountWithAny(node);

            return newNode ?? base.VisitBinaryExpression(node);
        }

        private void TrackChanges(SyntaxNode node)
        {
            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.UseMethodAnyMessage + " / " + SonarQube.RuleSpecification1155Message;
            ChangeTracker.Stage(new Message
            {
                Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message
            });
        }

        private ExpressionSyntax ReplaceCountWithAny(BinaryExpressionSyntax node)
        {
            switch (node.Left)
            {
                case BinaryExpressionSyntax left when node.Right is BinaryExpressionSyntax right:
                    {
                        var leftNode = VisitBinaryExpression(left);
                        var rightNode = VisitBinaryExpression(right);
                        return RewriteService.ConnectBinaryExpr(node, leftNode, rightNode,
                            node.OperatorToken.ValueText);
                    }
                case BinaryExpressionSyntax:
                    // Visit node.left at a later point
                    return null;
            }

            var countNode = node.DescendantNodes().OfType<IdentifierNameSyntax>()
                .First(n => n.Identifier.ValueText.Equals("Count"));

            if (node.Right is not LiteralExpressionSyntax rightExpr)
            {
                return null;
            }

            var rightValue = rightExpr.Token.Text;

            var op = node.OperatorToken.ValueText;

            var newNode = node.ReplaceNode(countNode, IdentifierName("Any").NormalizeWhitespace());
            var newAnyNode = newNode.DescendantNodes().OfType<IdentifierNameSyntax>()
                .FirstOrDefault(n => n.Identifier.ValueText.Equals("Any"));

            if (newAnyNode!.Parent is MemberAccessExpressionSyntax
                {Parent: not InvocationExpressionSyntax} memberAccess)
            {
                newNode = newNode.ReplaceNode(memberAccess,
                    InvocationExpression(memberAccess).NormalizeWhitespace());
            }

            var invocationNode = newNode.Left.NormalizeWhitespace();

            switch (op)
            {
                case OperatorToken.GreaterThan when "0".Equals(rightValue):
                case OperatorToken.GreaterThanOrEqual when "1".Equals(rightValue):
                case OperatorToken.NotEqual when "0".Equals(rightValue):
                    _usesLinqDirective = true;
                    TrackChanges(node);
                    return invocationNode;
                case OperatorToken.Equal when "0".Equals(rightValue):
                case OperatorToken.LessThanOrEqual when "0".Equals(rightValue):
                case OperatorToken.LessThan when "1".Equals(rightValue):
                    {
                        var invertedFix = PrefixUnaryExpression
                        (
                            SyntaxKind.LogicalNotExpression, invocationNode
                        ).NormalizeWhitespace();

                        _usesLinqDirective = true;
                        TrackChanges(node);

                        return invertedFix;
                    }

                default:
                    return null;
            }
        }
    }
}
