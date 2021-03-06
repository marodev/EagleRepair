using System;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using EagleRepair.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class UseStringInterpolationRewriterR9 : AbstractRewriter
    {
        public UseStringInterpolationRewriterR9(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.ArgumentList.Arguments.Count < 2)
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

            if (firstArgument is null || !firstArgument.StartsWith("\""))
            {
                // method is used with additional parameters --> string.Format(CultureInfo.CurrentCulture, ...)
                return base.VisitInvocationExpression(node);
            }

            SyntaxNode newStringInterpolationNode;
            try
            {
                newStringInterpolationNode = RewriteService.CreateInterpolatedString(node.ArgumentList.Arguments);
            }
            catch (Exception)
            {
                // TODO: catch more specific exception
                newStringInterpolationNode = null;
            }

            if (newStringInterpolationNode == null)
            {
                return base.VisitInvocationExpression(node);
            }

            newStringInterpolationNode = newStringInterpolationNode.NormalizeWhitespace();

            if (memberAccessExpr.Parent is not null)
            {
                // keep original trivia
                newStringInterpolationNode = newStringInterpolationNode.WithTriviaFrom(memberAccessExpr.Parent);
            }

            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.UseStringInterpolationMessage;
            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R9),
                LineNr = lineNumber,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = message,
                ReSharperId = ReSharperRule.UseStringInterpolation.ToString()
            });

            return newStringInterpolationNode;
        }
    }
}
