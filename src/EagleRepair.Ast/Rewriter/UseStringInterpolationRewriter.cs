using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class UseStringInterpolationRewriter : AbstractRewriter
    {
        public UseStringInterpolationRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
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

            var firstArgument = node.ArgumentList.Arguments.FirstOrDefault()?.ToString();

            if (firstArgument is null || firstArgument.StartsWith("CultureInfo."))
            {
                // method is used with additional parameters --> string.Format(CultureInfo.CurrentCulture, ...)
                return base.VisitInvocationExpression(node);
            }

            var newStringInterpolationNode = RewriteService.CreateInterpolatedString(node.ArgumentList.Arguments);

            if (newStringInterpolationNode == null)
            {
                return base.VisitInvocationExpression(node);
            }

            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.UseStringInterpolationMessage;
            ChangeTracker.Add(new Message {Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message});

            return newStringInterpolationNode;
        }
    }
}
