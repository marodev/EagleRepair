using Microsoft.CodeAnalysis;

namespace EagleRepair.Ast.Services
{
    public interface IDisplayService
    {
        public int GetLineNumber(SyntaxNode node);
    }
}
