using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using EagleRepair.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class UseCountPropertyRewriterR11 : AbstractRewriter
    {
        public UseCountPropertyRewriterR11(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(changeTracker, typeService,
            rewriteService, displayService)
        {
        }

        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.ArgumentList.Arguments.Any())
            {
                return base.VisitInvocationExpression(node);
            }

            if (node.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return base.VisitInvocationExpression(node);
            }

            if (!memberAccessExpressionSyntax.Name.ToString().Equals("Count"))
            {
                return base.VisitInvocationExpression(node);
            }

            var typeSymbol = SemanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression).Type;

            if (typeSymbol is null)
            {
                return base.VisitInvocationExpression(node);
            }

            var isArray = typeSymbol is IArrayTypeSymbol;

            var containingNamespace = typeSymbol
                .ContainingNamespace
                ?.ToDisplayString();

            if (!TypeService.InheritsFromIEnumerable(containingNamespace) && !isArray)
            {
                return base.VisitInvocationExpression(node);
            }

            // only consider collections that implement ICollection<T> or its derived interfaces.
            if (TypeService.IsIEnumerable(typeSymbol.ToString()))
            {
                return base.VisitInvocationExpression(node);
            }

            var newNode = node.Expression.WithTrailingTrivia(node.GetTrailingTrivia());

            if (newNode is not MemberAccessExpressionSyntax newMemberAccessExpr)
            {
                return node;
            }

            if (isArray)
            {
                newNode = newMemberAccessExpr.WithName(RewriteService.CreateIdentifier("Length"));
            }

            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.UseCollectionsCountProperty;
            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R11),
                LineNr = lineNumber,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = message,
                ReSharperId = ReSharperRule.UseCollectionsCountProperty.ToString()
            });

            return newNode;
        }
    }
}
