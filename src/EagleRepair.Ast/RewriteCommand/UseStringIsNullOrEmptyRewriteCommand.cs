using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class UseStringIsNullOrEmptyRewriteCommand : AbstractRewriteCommand
    {
        public UseStringIsNullOrEmptyRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }

        public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            return base.VisitBinaryExpression(node);
        }

        public override SyntaxNode? VisitBinaryPattern(BinaryPatternSyntax node)
        {
            return base.VisitBinaryPattern(node);
        }
    }
}
