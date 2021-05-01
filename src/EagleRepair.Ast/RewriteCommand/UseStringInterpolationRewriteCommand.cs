using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class UseStringInterpolationRewriteCommand : AbstractRewriteCommand
    {
        public UseStringInterpolationRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(
            changeTracker, typeService)
        {
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.ArgumentList.Arguments.Count() < 2)
            {
                // string.format has at least 2 arguments
                return base.VisitInvocationExpression(node);
            }

            if (node.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                // can't be string.format
                return base.VisitInvocationExpression(node);
            }

            if (!"string.format".Equals(memberAccessExpr.ToString().ToLower()))
            {
                return base.VisitInvocationExpression(node);
            }

            var newStringInterpolationNode = InjectUtils.CreateInterpolatedString(node.ArgumentList.Arguments);

            return newStringInterpolationNode ?? base.VisitInvocationExpression(node);
        }
    }
}
