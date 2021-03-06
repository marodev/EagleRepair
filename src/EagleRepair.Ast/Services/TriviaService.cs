using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast.Services
{
    public class TriviaService : ITriviaService
    {
        public SyntaxTriviaList ExtractTriviaToKeep(SyntaxTriviaList syntaxTriviaList)
        {
            var newLeadingTrivia = new List<SyntaxTrivia>();

            for (var i = 0; i < syntaxTriviaList.Count; i++)
            {
                if (IsComment(syntaxTriviaList[i]) || IsIfElseDirective(syntaxTriviaList[i]))
                {
                    newLeadingTrivia.Add(syntaxTriviaList[i]);
                    continue;
                }

                var oneLookAhead = i + 1;
                if (oneLookAhead >= syntaxTriviaList.Count)
                {
                    break;
                }

                if (syntaxTriviaList[i].IsKind(SyntaxKind.WhitespaceTrivia) &&
                    IsComment(syntaxTriviaList[oneLookAhead]))
                {
                    newLeadingTrivia.Add(syntaxTriviaList[i]);
                }
            }

            if (newLeadingTrivia.Any() && !IsIfElseDirective(newLeadingTrivia.Last()))
            {
                newLeadingTrivia.Add(SyntaxFactory.EndOfLine("\n"));
            }

            return new SyntaxTriviaList(newLeadingTrivia);
        }

        public bool IsIfElseDirective(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.IfDirectiveTrivia) ||
                   trivia.IsKind(SyntaxKind.ElseDirectiveTrivia);
        }

        private static bool IsComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                   trivia.IsKind(SyntaxKind.SingleLineCommentTrivia);
        }
    }
}
