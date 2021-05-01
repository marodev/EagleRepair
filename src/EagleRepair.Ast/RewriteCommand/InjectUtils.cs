using System;
using System.Collections.Generic;
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
                throw new ArgumentException("Refactor this method to support longer namespaces",
                    nameof(usingDirective));
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

        public static InvocationExpressionSyntax CreateOfTypeT(string variable, string type)
        {
            return InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, IdentifierName(variable),
                GenericName(
                        Identifier("OfType"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(type))))));
        }

        public static ExpressionStatementSyntax CreateNullPropagation(string variableName, string methodName,
            ArgumentListSyntax arguments = null)
        {
            var invocationExpr = InvocationExpression(
                MemberBindingExpression(
                    IdentifierName(methodName)));

            if (arguments != null)
            {
                invocationExpr = invocationExpr.WithArgumentList(arguments);
            }

            var expr = ExpressionStatement(
                ConditionalAccessExpression(IdentifierName(variableName),
                    invocationExpr
                ));

            return expr.NormalizeWhitespace();
        }

        private static InterpolatedStringTextSyntax CreateInterpolatedText(string text)
        {
            return InterpolatedStringText()
                .WithTextToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.InterpolatedStringTextToken,
                        text,
                        text,
                        TriviaList()));
        }

        private static InterpolationSyntax CreateInterpolation(ExpressionSyntax argument)
        {
            return Interpolation(argument);
        }

        public static InterpolatedStringExpressionSyntax CreateInterpolatedString(
            SeparatedSyntaxList<ArgumentSyntax> allArguments)
        {
            // first argument contains the full string
            var text = allArguments.FirstOrDefault()?.ToString();

            if (string.IsNullOrEmpty(text) || text.Length < 3 || allArguments.Count < 2)
            {
                return null;
            }

            // Remove first and last character, which are "
            text = text.Substring(1, text.Length - 2);
            // member accesses and method invocations
            var interpolationArguments = allArguments.Skip(1).ToList();
            var texts = new List<string>();
            var textParts = text.Split('{', '}');

            var position = 0;
            foreach (var part in textParts)
            {
                // ignore {0} ... {1} .. that occur in odd positions (except first and last position)
                if (position == 0 || position % 2 == 0 || position == part.Length - 1)
                {
                    texts.Add(part);
                }

                position++;
            }

            var interpolationContents = new List<InterpolatedStringContentSyntax>();
            var index = 0;
            foreach (var interpolationArgument in interpolationArguments)
            {
                var interpolatedString = CreateInterpolatedText(texts[index]);
                interpolationContents.Add(interpolatedString);

                var memberOrMethodCall = CreateInterpolation(interpolationArgument.Expression);
                interpolationContents.Add(memberOrMethodCall);
                index++;
            }

            interpolationContents.Add(CreateInterpolatedText(texts[index]));

            var expressionStmt =
                InterpolatedStringExpression(
                        Token(SyntaxKind.InterpolatedStringStartToken))
                    .WithContents(
                        List(interpolationContents));

            return expressionStmt;
        }

        public static PrefixUnaryExpressionSyntax CreateIsNotNullOrEmpty(string variableName)
        {
            return PrefixUnaryExpression(
                    SyntaxKind.LogicalNotExpression,
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                PredefinedType(
                                    Token(SyntaxKind.StringKeyword)),
                                IdentifierName("IsNullOrEmpty")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        IdentifierName(variableName))))));
        }
    }
}
