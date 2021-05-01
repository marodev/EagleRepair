using System;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class UseStringInterpolationRewriteCommand : AbstractRewriteCommand
    {
        public UseStringInterpolationRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            Console.WriteLine("foo");
            return base.VisitInvocationExpression(node);
        }
    }
}
