using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class NullChecksShouldNotBeUsedWithIsRewriteCommand : AbstractRewriteCommand
    {
        public NullChecksShouldNotBeUsedWithIsRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) :
            base(changeTracker, typeService)
        {
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.Left is not BinaryExpressionSyntax leftBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            // check if first (left) expression is s != null
            if (leftBinaryExpr.Left is not IdentifierNameSyntax leftIdentifierName)
            {
                return base.VisitBinaryExpression(node);
            }

            var leftOp = leftBinaryExpr.OperatorToken.ValueText;
            if (!leftOp.Equals("!=") && !leftOp.Equals("=="))
            {
                return base.VisitBinaryExpression(node);
            }

            if (leftBinaryExpr.Right is not LiteralExpressionSyntax nullLiteralExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            if (!nullLiteralExpr.Token.ToString().Equals("null"))
            {
                return base.VisitBinaryExpression(node);
            }

            var rightBinaryExpr = node.Right as BinaryExpressionSyntax;

            var rightIdentifier = "";
            if (rightBinaryExpr is not null)
            {
                rightIdentifier = ExtractIdentifier(rightBinaryExpr);
            }
            else
            {
                if (node.Right is not PrefixUnaryExpressionSyntax rightPrefixUnaryExpr)
                {
                    return base.VisitBinaryExpression(node);
                }

                if (rightPrefixUnaryExpr.Operand is not ParenthesizedExpressionSyntax rightOperandExpr)
                {
                    return base.VisitBinaryExpression(node);
                }

                if (rightOperandExpr.Expression is not BinaryExpressionSyntax rightBinaryExprInParenthesis)
                {
                    return base.VisitBinaryExpression(node);
                }

                rightIdentifier = ExtractIdentifier(rightBinaryExprInParenthesis);
            }

            if (string.IsNullOrEmpty(rightIdentifier))
            {
                return base.VisitBinaryExpression(node);
            }

            var leftIdentifier = leftIdentifierName.Identifier.ValueText;
            if (!leftIdentifier.Equals(rightIdentifier))
            {
                return base.VisitBinaryExpression(node);
            }

            if (node.Right is not PrefixUnaryExpressionSyntax unaryExpr)
            {
                return node.Right;
            }

            // use C# 9 !(s is string) --> s is not string
            var newNode = InjectUtils.ConvertUnaryToIsNotPattern(unaryExpr);
            return newNode;
        }

        private static string ExtractIdentifier(BinaryExpressionSyntax binaryExpr)
        {
            if (!binaryExpr.OperatorToken.ValueText.Equals("is"))
            {
                return null;
            }

            if (binaryExpr.Left is not IdentifierNameSyntax rightIdentifierName)
            {
                return null;
            }

            return rightIdentifierName.Identifier.ValueText;
        }
    }
}
