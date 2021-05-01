using System.Collections.Generic;
using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class SimplifyLinqRewriteCommand : AbstractRewriteCommand
    {
        public SimplifyLinqRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker,
            typeService)
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

            var nameSpace = ModelExtensions.GetTypeInfo(_semanticModel, invocationExpr).Type?.ContainingNamespace
                .ToString();

            if (!_typeService.InheritsFromIEnumerable(nameSpace))
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
            var whereNameSpace = ModelExtensions.GetTypeInfo(_semanticModel, variableIdentifier)
                .Type
                ?.ContainingNamespace?.ToString();

            if (!_typeService.InheritsFromIEnumerable(whereNameSpace))
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
                var newNode =
                    InjectUtils.CreateInvocation(variableName, invokedMethodName, invocationExpr.ArgumentList);
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

            var newOfTypeNode = InjectUtils.CreateOfTypeT(variableName, castedTypeInWhereCondition);

            return base.VisitInvocationExpression(newOfTypeNode);
        }
    }
}
