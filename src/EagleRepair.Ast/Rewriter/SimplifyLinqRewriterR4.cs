using System.Collections.Generic;
using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class SimplifyLinqRewriterR4 : AbstractRewriter
    {
        public SimplifyLinqRewriterR4(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(changeTracker,
            typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return base.VisitInvocationExpression(node);
            }

            var invokedMethodName = memberAccessExpr.Name.ToString();

            var linqKeyWords = new List<string>
            {
                "Any",
                "Count",
                "First",
                "FirstOrDefault",
                "Single",
                "SingleOrDefault",
                "Select",
                "Last"
            };
            if (!linqKeyWords.Contains(invokedMethodName))
            {
                return base.VisitInvocationExpression(node);
            }

            if (memberAccessExpr.Expression is not InvocationExpressionSyntax invocationExpr)
            {
                return base.VisitInvocationExpression(node);
            }

            var nameSpace = SemanticModel.GetTypeInfo(invocationExpr).Type?
                .ContainingNamespace?.ToString();

            if (!TypeService.InheritsFromIEnumerable(nameSpace))
            {
                return base.VisitInvocationExpression(node);
            }

            if (invocationExpr.Expression is not MemberAccessExpressionSyntax listWhereMemberAccessExpr)
            {
                return base.VisitInvocationExpression(node);
            }

            if (!listWhereMemberAccessExpr.Name.ToString().Equals("Where"))
            {
                return base.VisitInvocationExpression(node);
            }

            var variableIdentifier = listWhereMemberAccessExpr.Expression;
            var whereNameSpace = SemanticModel.GetTypeInfo(variableIdentifier)
                .Type
                ?.ContainingNamespace?.ToString();

            if (!TypeService.InheritsFromIEnumerable(whereNameSpace))
            {
                return base.VisitInvocationExpression(node);
            }

            if (variableIdentifier is not IdentifierNameSyntax variable)
            {
                return base.VisitInvocationExpression(node);
            }

            var variableName = variable.Identifier.ToString();

            if (!invokedMethodName.Equals("Select"))
            {
                var lambdaExpr =
                    invocationExpr.ArgumentList.Arguments.FirstOrDefault()?.Expression as
                        ParenthesizedLambdaExpressionSyntax;

                if (lambdaExpr?.ParameterList.Parameters.Count > 1)
                {
                    // .Any() takes only a Func<T, bool> as parameter,
                    // .Where has Func<T, T, bool> as well, e.g.  numbers.Where((t, i) => true).Any();
                    return base.VisitInvocationExpression(node);
                }

                var newNode =
                    RewriteService.CreateInvocation(variableName, invokedMethodName, invocationExpr.ArgumentList);

                var lineNumber = $"{DisplayService.GetLineNumber(node)}";
                var message = ReSharper.ReplaceWith(invokedMethodName) + " / " + SonarQube.RuleSpecification2971Message;
                ChangeTracker.Stage(new Message
                {
                    RuleId = nameof(Rule.R4),
                    LineNr = lineNumber,
                    FilePath = FilePath,
                    ProjectName = ProjectName,
                    Text = message
                });

                return base.VisitInvocationExpression(newNode);
            }

            var whereConditionLambdas =
                invocationExpr.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>().ToList();

            if (whereConditionLambdas.Count != 1)
            {
                return base.VisitInvocationExpression(node);
            }

            var lambdaBody = whereConditionLambdas.Single().Body;

            if (lambdaBody is not BinaryExpressionSyntax lambdaBinaryExpr)
            {
                return base.VisitInvocationExpression(node);
            }

            if (!"is".Equals(lambdaBinaryExpr.OperatorToken.ValueText))
            {
                return base.VisitInvocationExpression(node);
            }

            if (lambdaBinaryExpr.Right is not IdentifierNameSyntax)
            {
                base.VisitInvocationExpression(node);
            }

            var selectArguments = node.ArgumentList.Arguments;

            if (selectArguments.Count != 1)
            {
                base.VisitInvocationExpression(node);
            }

            var selectArgumentExpr = selectArguments.First().Expression;
            if (selectArgumentExpr is not SimpleLambdaExpressionSyntax)
            {
                base.VisitInvocationExpression(node);
            }

            var castedTypeInWhereCondition = ((IdentifierNameSyntax)lambdaBinaryExpr.Right).Identifier.ValueText;

            // selectLambdaExpr.Block;
            var bodyExpr = ((SimpleLambdaExpressionSyntax)selectArgumentExpr).Body;
            switch (bodyExpr)
            {
                case BinaryExpressionSyntax bodyExprAsType:
                    {
                        var op = bodyExprAsType.OperatorToken.ValueText;
                        if (op != "as")
                        {
                            return base.VisitInvocationExpression(node);
                        }

                        var castedTypeInSelect = bodyExprAsType.Right.ToString();
                        if (!castedTypeInSelect.Equals(castedTypeInWhereCondition))
                        {
                            return base.VisitInvocationExpression(node);
                        }

                        break;
                    }
                case CastExpressionSyntax bodyExprTypeCast:
                    {
                        var castedTypeInSelect = bodyExprTypeCast.Type.ToString();
                        if (!castedTypeInSelect.Equals(castedTypeInWhereCondition))
                        {
                            return base.VisitInvocationExpression(node);
                        }

                        break;
                    }
                default:
                    return base.VisitInvocationExpression(node);
            }

            var lineNr = $"{DisplayService.GetLineNumber(node)}";
            var msg = ReSharper.ReplaceWithOfType2Message + " / " + SonarQube.RuleSpecification2971Message;
            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R4),
                LineNr = lineNr,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = msg
            });

            var newOfTypeNode = RewriteService.CreateOfTypeT(variableName, castedTypeInWhereCondition);

            return base.VisitInvocationExpression(newOfTypeNode);
        }
    }
}
