using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EagleRepair.Ast.RewriteCommand
{
    public static class InjectUtils
    {
        public static CompilationUnitSyntax InjectUsingDirective(CompilationUnitSyntax compilation,
            string usingDirective)
        {
            var namespaces = usingDirective.Split(".");

            if (namespaces.Length != 2)
            {
                // might need to refactor this method to support longer namespaces
                throw new ArgumentException("Refactor this method to support longer namespaces", nameof(usingDirective));
            }

            var allUsings = compilation.Usings;
            var usingExists = allUsings.Any(u => u.Name.ToString().Equals(usingDirective));

            if (usingExists)
                // dont insert using directive as it already exists
            {
                return compilation;
            }

            var insertPos = 0; // position to insert using directive
            // find insert position
            foreach (var @using in allUsings)
            {
                var compareResult = string.Compare(@using.Name.ToString(), usingDirective);
                if (compareResult < 1)
                {
                    insertPos++;
                    continue;
                }


                break;
            }

            allUsings = allUsings.Insert(insertPos, CreateUsingDirective(namespaces[0], namespaces[1]));

            return compilation.WithUsings(allUsings);
        }

        private static UsingDirectiveSyntax CreateUsingDirective(string firstIdentifier, string secondIdentifier)
        {
            var usingDirective = UsingDirective
                (
                    QualifiedName
                        (
                            IdentifierName(firstIdentifier),
                            IdentifierName(secondIdentifier)
                        )
                        .WithDotToken
                        (
                            Token(SyntaxKind.DotToken)
                        )
                )
                .WithUsingKeyword
                (
                    Token
                    (
                        TriviaList(),
                        SyntaxKind.UsingKeyword,
                        TriviaList
                        (
                            Space
                        )
                    )
                ).WithSemicolonToken
                (
                    Token
                    (
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList
                        (
                            LineFeed
                        )
                    )
                );

            return usingDirective;
        }


        public static BinaryExpressionSyntax ConnectBinaryExpr(BinaryExpressionSyntax root, SyntaxNode left,
            SyntaxNode right, string op)
        {
            SyntaxKind parsedOp;

            try
            {
                parsedOp = ParseOp(op);
            }
            catch (ArgumentException ae)
            {
                // TODO: log exception
                return root;
            }

            if (left is ExpressionSyntax leftExpr && right is ExpressionSyntax rightExpr)
            {
                return BinaryExpression(parsedOp, leftExpr, rightExpr).NormalizeWhitespace();
            }

            return root;
        }

        private static SyntaxKind ParseOp(string op)
        {
            return op switch
            {
                "&&" => SyntaxKind.LogicalAndExpression,
                "||" => SyntaxKind.LogicalOrExpression,
                _ => throw new ArgumentException(nameof(op))
            };
        }

        public static SyntaxToken CreateIdentifier(string identifier)
        {
            return Identifier(identifier);
        }

        public static MemberAccessExpressionSyntax CreateMemberAccess(string variable, string methodName)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(variable),
                IdentifierName(methodName));
        }

        public static InvocationExpressionSyntax CreateInvocation(string variable, string methodName,
            ArgumentListSyntax arguments = null)
        {
            var invocation = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(variable),
                        IdentifierName(methodName)));

            if (arguments is null)
            {
                return invocation;
            }   
                
            return invocation.WithArgumentList(arguments);
        }

        public static IsPatternExpressionSyntax CreateIsPattern(string identifierName, string type, string designation)
        {
            return IsPatternExpression(
                IdentifierName(identifierName),
                DeclarationPattern(
                    IdentifierName(type),
                    SingleVariableDesignation(
                        Identifier(designation)))).NormalizeWhitespace();
        }
    }
}
