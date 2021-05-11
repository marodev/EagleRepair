using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
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
                var lineNumber = $"{DisplayService.GetLineNumber(node)}";
                var message = SonarQube.RuleSpecification4201Message + " / " + ReSharper.MergeSequentialChecksMessage;
                ChangeTracker.Stage(new Message
                {
                    Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message
                });
                return node.Right;
            }

            // use C# 9 !(s is string) --> s is not string
            var newNode = RewriteService.ConvertUnaryToIsNotPattern(unaryExpr);

            var lineNr = $"{DisplayService.GetLineNumber(node)}";
            var msg = SonarQube.RuleSpecification4201Message + " / " + ReSharper.MergeSequentialChecksMessage;
            ChangeTracker.Stage(new Message {Line = lineNr, Path = FilePath, Project = ProjectName, Text = msg});

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
