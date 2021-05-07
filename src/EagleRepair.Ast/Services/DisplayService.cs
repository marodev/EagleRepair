using Microsoft.CodeAnalysis;

namespace EagleRepair.Ast.Services
{
    public class DisplayService : IDisplayService
    {
        public int GetLineNumber(SyntaxNode node)
        {
            var span = node.SyntaxTree.GetLineSpan(node.Span);
            // StartLinePosition is zero based
            var lineNumber = span.StartLinePosition.Line + 1;
            return lineNumber;
        }
    }
}
