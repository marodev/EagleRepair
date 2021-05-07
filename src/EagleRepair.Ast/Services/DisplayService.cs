using Microsoft.CodeAnalysis;

namespace EagleRepair.Ast.Services
{
    public class DisplayService : IDisplayService
    {
        public int GetLineNumber(SyntaxNode node)
        {
            var span = node.SyntaxTree.GetLineSpan(node.Span);
            var lineNumber = span.StartLinePosition.Line;
            return lineNumber;
        }
    }
}
