using Microsoft.CodeAnalysis;

namespace EagleRepair.Ast.Services
{
    public interface ITriviaService
    {
        public SyntaxTriviaList ExtractTriviaToKeep(SyntaxTriviaList syntaxTriviaList);
        public bool IsIfElseDirective(SyntaxTrivia trivia);
    }
}
