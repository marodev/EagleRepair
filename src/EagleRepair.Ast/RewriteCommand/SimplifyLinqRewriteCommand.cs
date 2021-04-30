using System;
using System.Collections.Generic;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class SimplifyLinqRewriteCommand : AbstractRewriteCommand
    {
        public SimplifyLinqRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return base.VisitInvocationExpression(node);
            }

            var invokedMethodName = memberAccessExpr.Name.ToString();

            var linqKeyWords = new List<string> {"Any", "Count", "First", "FirstOrDefault", "Single", "SingleOrDefault"};
            if (!linqKeyWords.Contains(invokedMethodName))
            {
                return base.VisitInvocationExpression(node);
            }

            if (memberAccessExpr.Expression is not InvocationExpressionSyntax invocationExpr)
            {
                return base.VisitInvocationExpression(node);
            }

            var nameSpace = _semanticModel.GetTypeInfo(invocationExpr).Type?.ContainingNamespace.ToString();

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
            var whereNameSpace = _semanticModel.GetTypeInfo(variableIdentifier)
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

            var newNode = InjectUtils.CreateInvocation(variableName, invokedMethodName, invocationExpr.ArgumentList);
            
            return base.VisitInvocationExpression(newNode);
        }
    }
}
