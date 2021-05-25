using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class NullChecksShouldNotBeUsedWithIsRewriterR3 : AbstractRewriter
    {
        public NullChecksShouldNotBeUsedWithIsRewriterR3(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) :
            base(changeTracker, typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.Left is not BinaryExpressionSyntax leftBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            var leftIdentifierName = leftBinaryExpr.Left as IdentifierNameSyntax;
            var leftMemberAccessExpr = leftBinaryExpr.Left as MemberAccessExpressionSyntax;

            if (leftIdentifierName is null && leftMemberAccessExpr is null)
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
            else if (node.Right is IsPatternExpressionSyntax rightIsPatternExpr)
            {
                rightIdentifier = rightIsPatternExpr.Expression.ToString();
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

            var leftIdentifier = leftIdentifierName?.Identifier.ValueText ?? leftMemberAccessExpr.ToString();
            if (!leftIdentifier.Equals(rightIdentifier))
            {
                return base.VisitBinaryExpression(node);
            }

            if (node.Right is not PrefixUnaryExpressionSyntax unaryExpr)
            {
                if (node.OperatorToken.Kind() == SyntaxKind.BarBarToken)
                {
                    // we can't refactor something like (s == null || s is string) to (s is string)
                    return base.VisitBinaryExpression(node);
                }

                var lineNumber = $"{DisplayService.GetLineNumber(node)}";
                var message = SonarQube.RuleSpecification4201Message + " / " + ReSharper.MergeSequentialChecksMessage;
                ChangeTracker.Stage(new Message
                {
                    RuleId = nameof(Rule.R3),
                    LineNr = lineNumber,
                    FilePath = FilePath,
                    ProjectName = ProjectName,
                    Text = message
                });
                return node.Right;
            }

            // TODO: only if target is NET5.0
            // use C# 9 !(s is string) --> s is not string
            if (leftBinaryExpr.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
            {
                // we can't fix a pattern such as --> s != null && !(s is string)
                return base.VisitBinaryExpression(node);
            }

            // TODO: if target is dotnet 5.0, use "is not null" instead of !(... == null)
            // var newNode = RewriteService.ConvertUnaryToIsNotPattern(unaryExpr);

            var lineNr = $"{DisplayService.GetLineNumber(node)}";
            var msg = SonarQube.RuleSpecification4201Message + " / " + ReSharper.MergeSequentialChecksMessage;
            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R3),
                LineNr = lineNr,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = msg
            });

            return unaryExpr;
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
