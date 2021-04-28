using System.Linq;
using EagleRepair.Ast.ReservedToken;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EagleRepair.Ast.RewriteCommand
{
    public class UseMethodAnyRewriteCommand : AbstractRewriteCommand
    {
        private bool _usesLinqDirective;

        public UseMethodAnyRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) :
            base(changeTracker, typeService)
        {
        }


        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var resultNode = base.VisitCompilationUnit(node);

            if (!_usesLinqDirective)
            {
                return resultNode;
            }

            return InjectUtils.InjectUsingDirective((CompilationUnitSyntax)resultNode, "System.Linq");
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

            var typeSymbol = _semanticModel.GetTypeInfo(countNode.Parent.ChildNodes().ElementAt(0))
                .Type;

            if (typeSymbol == null)
            {
                return base.VisitBinaryExpression(node);
            }

            var containingNamespace = typeSymbol
                .ContainingNamespace
                .ToDisplayString();

            if (string.IsNullOrEmpty(containingNamespace))
            {
                return base.VisitBinaryExpression(node);
            }

            if (!_typeService.InheritsFromIEnumerable(containingNamespace))
            {
                return base.VisitBinaryExpression(node);
            }


            return ReplaceCountWithAny(node);
        }

        private ExpressionSyntax ReplaceCountWithAny(BinaryExpressionSyntax node)
        {
            var countNode = node.DescendantNodes().OfType<IdentifierNameSyntax>()
                .FirstOrDefault(n => n.Identifier.ValueText.Equals("Count"));

            var rightExpr = (LiteralExpressionSyntax)node.Right;
            var rightValue = rightExpr.Token.Text;

            var op = node.OperatorToken.ValueText;
            // var op = condition.OperatorToken.ValueText;
            var location = countNode.GetLocation();

            var newNode = node.ReplaceNode(countNode, IdentifierName("Any").NormalizeWhitespace());
            var newAnyNode = newNode.DescendantNodes().OfType<IdentifierNameSyntax>()
                .FirstOrDefault(n => n.Identifier.ValueText.Equals("Any"));

            if (newAnyNode!.Parent is MemberAccessExpressionSyntax memberAccess)
            {
                if (!(memberAccess.Parent is InvocationExpressionSyntax))
                {
                    newNode = newNode.ReplaceNode(memberAccess,
                        InvocationExpression(memberAccess).NormalizeWhitespace());
                }
            }

            var invocationNode = newNode.Left.NormalizeWhitespace();

            switch (op)
            {
                case OperatorToken.GreaterThan when "0".Equals(rightValue):
                case OperatorToken.GreaterThanOrEqual when "1".Equals(rightValue):
                    _usesLinqDirective = true;
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
                        return invertedFix;
                    }
                default:
                    return node;
            }
        }
    }
}
