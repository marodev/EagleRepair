using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class MergeSequentialChecksRewriteCommand : AbstractRewriteCommand
    {
        public MergeSequentialChecksRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(
            changeTracker, typeService)
        {
        }

        private static ExpressionSyntax FixRightBinaryExpr(string leftExprVariableName,
            BinaryExpressionSyntax binaryExpr)
        {
            if (binaryExpr.Left is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return null;
            }

            if (memberAccessExpr.Expression is not IdentifierNameSyntax identifierName)
            {
                return null;
            }

            var variableName = identifierName.ToString();
            if (!leftExprVariableName.Equals(variableName))
            {
                return null;
            }

            var invokedMemberName = memberAccessExpr.Name.ToString();

            var op = binaryExpr.OperatorToken.ToString();
            switch (binaryExpr.Right)
            {
                case LiteralExpressionSyntax literalExpr when !literalExpr.ToString().Equals("null"):
                    return null;
                case LiteralExpressionSyntax:
                    return InjectUtils.CreateNullPatternExprWithConditionalMemberAccess(variableName,
                        op, invokedMemberName);
                case IdentifierNameSyntax rightIdentifierName:
                    {
                        var targetType = rightIdentifierName.Identifier.ToString();
                        return InjectUtils.CreateIsTypePatternExprWithConditionalMemberAccess(variableName,
                            invokedMemberName, op, targetType);
                    }
                default:
                    return null;
            }
        }

        private static ExpressionSyntax FixRightIsPatternExpr(string leftExprVariableName,
            IsPatternExpressionSyntax isPatternExpr)
        {
            if (isPatternExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return null;
            }

            PatternSyntax declarationOrConstNullPattern;
            switch (isPatternExpr.Pattern)
            {
                case DeclarationPatternSyntax declarationPattern:
                    declarationOrConstNullPattern = declarationPattern;
                    break;
                case ConstantPatternSyntax constantPattern:
                    declarationOrConstNullPattern = constantPattern;
                    break;
                default:
                    return null;
            }

            if (memberAccessExpr.Name is not IdentifierNameSyntax invokedMemberName)
            {
                return null;
            }

            if (memberAccessExpr.Expression is not IdentifierNameSyntax invokedMemberIdentifierName)
            {
                return null;
            }

            var variableName = invokedMemberIdentifierName.Identifier.ToString();
            if (!leftExprVariableName.Equals(variableName))
            {
                return null;
            }

            var opKeyword = isPatternExpr.IsKeyword.ToString();

            IsPatternExpressionSyntax newNode = null;
            switch (declarationOrConstNullPattern)
            {
                case DeclarationPatternSyntax declPattern:
                    newNode = InjectUtils.CreateIsPatternExprWithConditionalMemberAccessAndDeclaration(variableName,
                        opKeyword,
                        invokedMemberName.ToString(), declPattern.Type.ToString(),
                        declPattern.Designation.ToString()).NormalizeWhitespace();
                    break;
                case ConstantPatternSyntax constantPattern:
                    {
                        if (constantPattern.Expression is not LiteralExpressionSyntax literalExpr)
                        {
                            return null;
                        }

                        if (!literalExpr.ToString().Equals("null"))
                        {
                            return null;
                        }

                        newNode = InjectUtils.CreateNullPatternExprWithConditionalMemberAccess(variableName, opKeyword,
                            invokedMemberName.ToString());
                        break;
                    }
            }

            return newNode;
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var allBinaryExprCount = node.DescendantNodes().OfType<BinaryExpressionSyntax>().Count();

            if (allBinaryExprCount != 2 && node.Right is not IsPatternExpressionSyntax)
            {
                return base.VisitBinaryExpression(node);
            }

            if (node.Left is not BinaryExpressionSyntax leftBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            if (leftBinaryExpr.Left is not IdentifierNameSyntax leftLeftIdentifierName)
            {
                return base.VisitBinaryExpression(node);
            }

            var leftExprVariableName = leftLeftIdentifierName.ToString();
            var newNode = node.Right switch
            {
                BinaryExpressionSyntax rightBinaryExpr => FixRightBinaryExpr(leftExprVariableName, rightBinaryExpr),
                IsPatternExpressionSyntax isPatternExpr => FixRightIsPatternExpr(leftExprVariableName, isPatternExpr),
                _ => null
            };

            if (newNode is null)
            {
                return base.VisitBinaryExpression(node);
            }

            // keep original space
            newNode = newNode.WithTrailingTrivia(node.Right.GetTrailingTrivia());

            return newNode;
        }
    }
}
