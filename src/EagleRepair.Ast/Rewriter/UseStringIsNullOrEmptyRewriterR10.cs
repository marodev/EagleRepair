using EagleRepair.Ast.ReservedToken;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class UseStringIsNullOrEmptyRewriterR10 : AbstractRewriter
    {
        public UseStringIsNullOrEmptyRewriterR10(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
        }

        private SyntaxNode ExtractIsNullOrEmpty(InvocationExpressionSyntax node)
        {
            if (node.ArgumentList.Arguments.Count != 1)
            {
                return null;
            }

            var firstArgument = node.ArgumentList.Arguments.First();

            if (firstArgument.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return null;
            }

            if (!memberAccessExpressionSyntax.Name.ToString().Equals("Empty"))
            {
                return null;
            }

            if (!memberAccessExpressionSyntax.OperatorToken.IsKind(SyntaxKind.DotToken))
            {
                return null;
            }

            var stringIdentifier = memberAccessExpressionSyntax.Expression;

            if (!stringIdentifier.IsKind(SyntaxKind.PredefinedType))
            {
                return null;
            }

            if (!stringIdentifier.ToString().Equals("string"))
            {
                return null;
            }

            if (node.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return null;
            }

            if (memberAccessExpr.Expression is not IdentifierNameSyntax identifier)
            {
                return null;
            }

            return RewriteService.CreateIsNullOrEmpty(identifier.ToString());
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var newNode = ExtractIsNullOrEmpty(node);

            if (newNode is null)
            {
                return base.VisitInvocationExpression(node);
            }

            AddNodeToMonitor(newNode);
            return newNode;
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var op = node.OperatorToken.ToString();

            if (!op.Equals("&&") && !op.Equals("||"))
            {
                return base.VisitBinaryExpression(node);
            }

            // we are looking for a binary expr that has 2 children, which are as well binary expressions
            if (node.Left is not BinaryExpressionSyntax subLeftBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            if (node.Right is not BinaryExpressionSyntax subRightBinaryExpr)
            {
                if (node.Right is not (InvocationExpressionSyntax or PrefixUnaryExpressionSyntax))
                {
                    return base.VisitBinaryExpression(node);
                }

                if (node.Left is not BinaryExpressionSyntax leftBinaryExpr)
                {
                    return base.VisitBinaryExpression(node);
                }

                SyntaxNode newSyntaxNode;
                if (node.Right is InvocationExpressionSyntax invocationExpr)
                {
                    newSyntaxNode = ExtractIsNullOrEmpty(invocationExpr);
                }
                else
                {
                    var prefixUnaryExpr = node.Right as PrefixUnaryExpressionSyntax;
                    var invocOp = prefixUnaryExpr?.Operand;

                    if (invocOp is not InvocationExpressionSyntax syntax)
                    {
                        return base.VisitBinaryExpression(node);
                    }

                    invocationExpr = syntax;
                    var newStringIsNullOrEmptyNode = ExtractIsNullOrEmpty(syntax);

                    if (newStringIsNullOrEmptyNode is null)
                    {
                        return base.VisitBinaryExpression(node);
                    }

                    if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
                    {
                        return base.VisitBinaryExpression(node);
                    }

                    var variableName = memberAccessExpr.Expression.ToString();

                    newSyntaxNode = RewriteService.CreateIsNotNullOrEmpty(variableName);
                }

                if (newSyntaxNode is null)
                {
                    return base.VisitBinaryExpression(node);
                }

                if (leftBinaryExpr.Left is not IdentifierNameSyntax subLeftIdentifierName)
                {
                    return base.VisitBinaryExpression(node);
                }

                if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
                {
                    return base.VisitBinaryExpression(node);
                }

                // TODO: code duplication - cleanup
                if (memberAccessExpressionSyntax.Expression is not IdentifierNameSyntax rightIdentifierNme)
                {
                    return base.VisitBinaryExpression(node);
                }

                var rightId = rightIdentifierNme.Identifier.ToString();
                var leftIdentifier = subLeftIdentifierName.Identifier.ToString();

                if (!leftIdentifier.Equals(rightId))
                {
                    return base.VisitBinaryExpression(node);
                }

                AddNodeToMonitor(node);
                return newSyntaxNode;
            }

            if (subRightBinaryExpr.Right is not LiteralExpressionSyntax)
            {
                return base.VisitBinaryExpression(node);
            }

            // check if first (left) expression is s != null
            if (subLeftBinaryExpr.Left is not IdentifierNameSyntax leftIdentifierName)
            {
                return base.VisitBinaryExpression(node);
            }

            if (!subLeftBinaryExpr.OperatorToken.ValueText.Equals("!="))
            {
                return base.VisitBinaryExpression(node);
            }

            if (subLeftBinaryExpr.Right is not LiteralExpressionSyntax nullLiteralExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            if (!nullLiteralExpr.Token.ToString().Equals("null"))
            {
                return base.VisitBinaryExpression(node);
            }

            var rightIdentifier = subRightBinaryExpr.Left;
            IdentifierNameSyntax rightIdentifierName;

            switch (rightIdentifier)
            {
                case IdentifierNameSyntax identifierName:
                    {
                        // check if s != ""
                        if (!"!=".Equals(subRightBinaryExpr.OperatorToken.ToString()))
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        if (subRightBinaryExpr.Right is not LiteralExpressionSyntax literalExpr)
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        if (!literalExpr.Token.ValueText.Equals(""))
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        rightIdentifierName = identifierName;
                        break;
                    }
                case MemberAccessExpressionSyntax memberAccess when !memberAccess.Name.ToString().Equals("Length"):
                    return base.VisitBinaryExpression(node);
                case MemberAccessExpressionSyntax memberAccess:
                    {
                        rightIdentifierName = memberAccess.Expression as IdentifierNameSyntax;

                        if (rightIdentifierName is null)
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        if (subRightBinaryExpr.Right is not LiteralExpressionSyntax valueLiteralExpr)
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        var numericalToken = valueLiteralExpr.Token.Value;

                        if (numericalToken is not int intToken)
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        var opText = subRightBinaryExpr.OperatorToken.Text;
                        switch (intToken)
                        {
                            case 0:
                                {
                                    // operator must be >
                                    if (!opText.Equals(OperatorToken.GreaterThan))
                                    {
                                        return base.VisitBinaryExpression(node);
                                    }

                                    break;
                                }
                            case 1:
                                {
                                    // operator must be >=
                                    if (!opText.Equals(OperatorToken.GreaterThanOrEqual))
                                    {
                                        return base.VisitBinaryExpression(node);
                                    }

                                    break;
                                }
                            default:
                                return base.VisitBinaryExpression(node);
                        }

                        break;
                    }
                default:
                    return base.VisitBinaryExpression(node);
            }

            if (!leftIdentifierName.ToString().Equals(rightIdentifierName.ToString()))
            {
                return base.VisitBinaryExpression(node);
            }

            var leftSymbol = ModelExtensions.GetTypeInfo(SemanticModel, leftIdentifierName).Type?.ToString();
            var rightSymbol = ModelExtensions.GetTypeInfo(SemanticModel, rightIdentifierName).Type?.ToString();

            // must be of type string, otherwise string.IsNullOrEmpty(..) can't be used.
            if (!"string".Equals(leftSymbol) || !leftSymbol.Equals(rightSymbol))
            {
                return base.VisitBinaryExpression(node);
            }

            var newNode = RewriteService.CreateIsNotNullOrEmpty(leftIdentifierName.ToString());

            AddNodeToMonitor(newNode);

            // keep original space after node
            newNode = newNode.WithTrailingTrivia(node.GetTrailingTrivia());
            return newNode;
        }

        private void AddNodeToMonitor(SyntaxNode node)
        {
            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.ReplaceWithStringIsNullOrEmptyMessage + " / " +
                          SonarQube.RuleSpecification3256Message;

            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R10),
                LineNr = lineNumber,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = message
            });
        }
    }
}
