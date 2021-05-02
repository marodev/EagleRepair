using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class MergeSequentialChecksRewriteCommand : AbstractRewriteCommand
    {
        public MergeSequentialChecksRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }

        public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            
            return base.VisitBinaryExpression(node);
        }
    }
}
