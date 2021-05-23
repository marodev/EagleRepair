using System;
using System.Collections.Generic;
using System.Linq;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EagleRepair.Ast.Services
{
    public class RewriteService : IRewriteService
    {
        private readonly IChangeTracker _monitor;

        public RewriteService(IChangeTracker monitor)
        {
            _monitor = monitor;
        }

        public CompilationUnitSyntax InjectUsingDirective(CompilationUnitSyntax compilation,
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


        public BinaryExpressionSyntax ConnectBinaryExpr(BinaryExpressionSyntax root, SyntaxNode left,
            SyntaxNode right, string op)
        {
            SyntaxKind parsedOp;

            try
            {
                parsedOp = ParseOp(op);
            }
            catch (ArgumentException)
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

        public MemberAccessExpressionSyntax CreateMemberAccess(string variable, string methodName)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(variable),
                IdentifierName(methodName));
        }

        public InvocationExpressionSyntax CreateInvocation(string variable, string methodName,
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

        public IsPatternExpressionSyntax CreateIsPattern(ExpressionSyntax identifierName, TypeSyntax type,
            string designation, SyntaxNode syntaxTrivia = null)
        {
            var newIdentifierName = identifierName;
            var identifierAnnotation = new SyntaxAnnotation("identifierAnnotation");

            IsPatternExpressionSyntax patternExpr = null;

            if (identifierName is InvocationExpressionSyntax)
            {
                newIdentifierName = newIdentifierName.WithAdditionalAnnotations(identifierAnnotation);
            }

            patternExpr = IsPatternExpression(
                newIdentifierName,
                DeclarationPattern(
                    type,
                    SingleVariableDesignation(
                        Identifier(designation))
                )).NormalizeWhitespace();

            if (syntaxTrivia != null)
            {
                patternExpr = patternExpr.WithTriviaFrom(syntaxTrivia);
            }

            var annotatedNode = patternExpr.GetAnnotatedNodes(identifierAnnotation).FirstOrDefault();

            if (annotatedNode is null)
            {
                return patternExpr;
            }

            // .NormalizeWhitespace() does not work in case there is a method invocation
            // we manually add the missing whitespace here
            patternExpr = patternExpr.Update(identifierName.WithTrailingTrivia(TriviaList(Space)),
                patternExpr.IsKeyword, patternExpr.Pattern);

            return patternExpr;
        }

        public InvocationExpressionSyntax CreateOfTypeT(string variable, string type)
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

        public ExpressionStatementSyntax CreateNullPropagation(string variableName, string methodName,
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

        public InterpolatedStringExpressionSyntax CreateInterpolatedString(
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
            var interpolationFormats = new List<InterpolationFormatClauseSyntax>();
            var textParts = text.Split('{', '}');

            var position = 0;
            foreach (var part in textParts)
            {
                // ignore {0} ... {1} .. that occur in odd positions (except first and last position)
                if (position == 0 || position % 2 == 0 || position == part.Length - 1)
                {
                    texts.Add(part);
                }
                else
                {
                    // something in braces {0}
                    // Try to get format, e.g. {0:2N}
                    try
                    {
                        var format = GetInterpolatedFormat(part);
                        interpolationFormats.Add(format);
                    }
                    catch (ArgumentException)
                    {
                        // failed to parse format, abort
                        return null;
                    }
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

                var format = interpolationFormats[index];

                if (format != null)
                {
                    memberOrMethodCall = memberOrMethodCall.WithFormatClause(format);
                }

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

        public PrefixUnaryExpressionSyntax CreateIsNotNullOrEmpty(string variableName)
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
                            SingletonSeparatedList(
                                Argument(
                                    IdentifierName(variableName))))));
        }

        public ExpressionSyntax ConvertUnaryToIsNotPattern(PrefixUnaryExpressionSyntax unaryExpr)
        {
            var binaryExpr = unaryExpr.DescendantNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();

            if (binaryExpr is null)
            {
                return unaryExpr;
            }

            return IsPatternExpression
            (
                binaryExpr.Left,
                UnaryPattern
                (
                    ConstantPattern
                    (
                        binaryExpr.Right
                    )
                )
            ).NormalizeWhitespace();
        }

        public ExpressionSyntax CreateConditionalAccess(string variableName, string memberName)
        {
            return ConditionalAccessExpression
            (
                IdentifierName(variableName),
                MemberBindingExpression
                (
                    IdentifierName(memberName)
                )
            );
        }

        public ExpressionSyntax CreateNullPatternExprWithConditionalMemberAccess(string variableName,
            string op, string memberName)
        {
            if (!op.Equals("!=") && !op.Equals("=="))
            {
                return IsPatternExpression
                (
                    CreateConditionalAccess(variableName, memberName),
                    CreateNullPattern(op)
                ).NormalizeWhitespace();
            }

            var syntaxOp = SyntaxKind.EqualsExpression;
            if (op.Equals("!="))
            {
                syntaxOp = SyntaxKind.NotEqualsExpression;
            }

            return BinaryExpression(
                syntaxOp,
                CreateConditionalAccess(variableName, memberName),
                LiteralExpression(
                    SyntaxKind.NullLiteralExpression)).NormalizeWhitespace();
        }

        public IsPatternExpressionSyntax CreateIsPatternExprWithConditionalMemberAccessAndDeclaration(
            string variableName, string op, string memberName, string targetTypeName, string declarationName)
        {
            return IsPatternExpression(
                CreateConditionalAccess(variableName, memberName),
                CreateDeclarationPattern(targetTypeName, declarationName, op));
        }

        public IsPatternExpressionSyntax CreateIsTypePatternExprWithConditionalMemberAccess(string variableName,
            string memberName, string op, string targetTypeName)
        {
            return IsPatternExpression(
                CreateConditionalAccess(variableName, memberName),
                CreateConstant(targetTypeName, op)).NormalizeWhitespace();
        }

        public SyntaxNode AddSealedKeyword(ClassDeclarationSyntax classDecl)
        {
            var modifiers = classDecl.Modifiers;
            var firstModifier = modifiers.FirstOrDefault();
            var sealedToken = Token(SyntaxKind.SealedKeyword)
                .WithTrailingTrivia(firstModifier.TrailingTrivia);

            modifiers = modifiers.Add(sealedToken);
            var newClass = classDecl.WithModifiers(modifiers);
            return newClass;
        }

        ClassDeclarationSyntax IRewriteService.ModifyDisposeAndAddProtectedDispose(ClassDeclarationSyntax classDecl,
            MethodDeclarationSyntax disposeMethodDecl)
        {
            return ModifyDisposeAndAddProtectedDispose(classDecl, disposeMethodDecl);
        }

        public ExpressionSyntax CreateConditionalBinaryExpr(string variableName, string memberName, SyntaxToken op,
            ExpressionSyntax rightExpr)
        {
            return BinaryExpression(
                SyntaxFacts.GetBinaryExpression(op.Kind()),
                ConditionalAccessExpression(
                    IdentifierName(variableName),
                    MemberBindingExpression(
                        IdentifierName(memberName))),
                rightExpr).NormalizeWhitespace();
        }

        private static InterpolationFormatClauseSyntax GetInterpolatedFormat(string part)
        {
            InterpolationFormatClauseSyntax interpolationFormat;

            var formats = part.Split(":");
            if (formats.Length > 1)
            {
                interpolationFormat = InterpolationFormatClause(
                    Token(SyntaxKind.ColonToken));
            }
            else
            {
                formats = part.Split(",");
                interpolationFormat = InterpolationFormatClause(
                    Token(SyntaxKind.CommaToken));
            }

            if (formats.Length <= 1)
            {
                return null;
            }

            var formatString = string.Join("", formats.Skip(1).ToArray());

            if (!formatString.ToCharArray().Any(char.IsDigit))
            {
                throw new ArgumentException("format should contain a number.");
            }

            if (formatString.Contains("(") && formatString.Contains(")") && formatString.Contains("+"))
            {
                throw new ArgumentException("can't format a concatenated string.");
            }

            interpolationFormat = interpolationFormat.WithFormatStringToken(Token(
                TriviaList(),
                SyntaxKind.InterpolatedStringTextToken,
                formatString,
                formatString,
                TriviaList()));

            return interpolationFormat;
        }

        private UsingDirectiveSyntax CreateUsingDirective(string firstIdentifier, string secondIdentifier)
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

        private static SyntaxKind ParseOp(string op)
        {
            return op switch
            {
                "&&" => SyntaxKind.LogicalAndExpression,
                "||" => SyntaxKind.LogicalOrExpression,
                _ => throw new ArgumentException(nameof(op))
            };
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
            return Interpolation(argument is ConditionalExpressionSyntax
                ? ParenthesizedExpression(argument)
                : argument);
        }

        private static PatternSyntax CreateNullLiteral()
        {
            return ConstantPattern
            (
                LiteralExpression
                (
                    SyntaxKind.NullLiteralExpression
                )
            );
        }

        private static PatternSyntax CreateNullPattern(string op)
        {
            if (op.Equals("!=") || op.Equals("is not"))
            {
                return UnaryPattern
                (
                    CreateNullLiteral()
                );
            }

            return CreateNullLiteral();
        }

        private static PatternSyntax CreateDeclarationPattern(string targetTypeName, string declarationName, string op)
        {
            var declaration = DeclarationPattern(
                IdentifierName(targetTypeName),
                SingleVariableDesignation(
                    Identifier(declarationName)));

            if (!op.Equals("!=") || !op.Equals("is not"))
            {
                return declaration;
            }

            return UnaryPattern(declaration);
        }

        private static PatternSyntax CreateConstant(string variableName, string op)
        {
            var constPattern = ConstantPattern(IdentifierName(variableName));

            if (op.Equals("is not"))
            {
                return UnaryPattern(constPattern);
            }

            return constPattern;
        }

        private static MethodDeclarationSyntax CreateVoidMethod(SyntaxTokenList modifiers, string methodName,
            ParameterListSyntax parameters, BlockSyntax body)
        {
            return MethodDeclaration(
                    PredefinedType(
                        Token(SyntaxKind.VoidKeyword)),
                    Identifier(methodName))
                .WithModifiers(TokenList(modifiers))
                .WithParameterList(parameters)
                .WithBody(body);
        }

        private static MethodDeclarationSyntax CreateProtectedDisposeMethod(BlockSyntax ifBlockStmts,
            SyntaxTriviaList? leadingTrivia)
        {
            return MethodDeclaration(
                    PredefinedType(
                        Token(SyntaxKind.VoidKeyword)),
                    Identifier("Dispose"))
                .WithModifiers(
                    TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.VirtualKeyword)))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(
                                    Identifier("disposing"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.BoolKeyword))))))
                .WithBody(
                    Block(
                            IfStatement(
                                IdentifierName("_disposed"),
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement()))),
                            IfStatement(
                                IdentifierName("disposing"),
                                ifBlockStmts
                            ),
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_disposed"),
                                    LiteralExpression(
                                        SyntaxKind.TrueLiteralExpression)))).NormalizeWhitespace()
                        .WithLeadingTrivia(leadingTrivia)
                ).WithLeadingTrivia(TriviaList
                (
                    LineFeed));
        }

        private static BlockSyntax CreateDisposeMethodBlock()
        {
            return Block(
                ExpressionStatement(
                        InvocationExpression(
                                IdentifierName("Dispose"))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.TrueLiteralExpression))))))
                    .WithSemicolonToken(
                        Token(TriviaList(), SyntaxKind.SemicolonToken, TriviaList(LineFeed))
                    ),
                ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("GC"),
                                    IdentifierName("SuppressFinalize")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            ThisExpression())))))
                    .WithSemicolonToken
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
                    )
            ).NormalizeWhitespace();
        }

        private static ExpressionStatementSyntax GetGcInvocation(SyntaxNode disposeMethodDecl)
        {
            return disposeMethodDecl
                .DescendantNodes()
                .OfType<ExpressionStatementSyntax>()
                .FirstOrDefault(e =>
                    e.Expression is InvocationExpressionSyntax
                    {
                        Expression: MemberAccessExpressionSyntax
                            {Expression: IdentifierNameSyntax identifierName}
                    } && identifierName.ToString().Equals("GC"));
        }

        private static FieldDeclarationSyntax CreateDisposeField()
        {
            return FieldDeclaration
                (
                    VariableDeclaration
                        (
                            PredefinedType
                            (
                                Token
                                (
                                    TriviaList(),
                                    SyntaxKind.BoolKeyword,
                                    TriviaList
                                    (
                                        Space
                                    )
                                )
                            )
                        )
                        .WithVariables
                        (
                            SingletonSeparatedList
                            (
                                VariableDeclarator
                                    (
                                        Identifier
                                        (
                                            TriviaList(),
                                            "_disposed",
                                            TriviaList
                                            (
                                                Space
                                            )
                                        )
                                    )
                                    .WithInitializer
                                    (
                                        EqualsValueClause
                                            (
                                                LiteralExpression
                                                (
                                                    SyntaxKind.FalseLiteralExpression
                                                )
                                            )
                                            .WithEqualsToken
                                            (
                                                Token
                                                (
                                                    TriviaList(),
                                                    SyntaxKind.EqualsToken,
                                                    TriviaList
                                                    (
                                                        Space
                                                    )
                                                )
                                            )
                                    )
                            )
                        )
                )
                .WithModifiers
                (
                    TokenList
                    (
                        Token
                        (
                            TriviaList
                                (Comment("// To detect redundant calls"), LineFeed),
                            SyntaxKind.PrivateKeyword,
                            TriviaList
                            (
                                Space
                            )
                        )
                    )
                )
                .WithSemicolonToken
                (
                    Token
                    (
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList
                        (
                            LineFeed,
                            LineFeed
                        )
                    )
                );
        }

        private static SyntaxTriviaList? GetLeadingBodyTrivia(MethodDeclarationSyntax methodDecl)
        {
            var firstExpr = methodDecl.Body?.GetLeadingTrivia();
            return firstExpr;
        }

        public ClassDeclarationSyntax ModifyDisposeAndAddProtectedDispose(ClassDeclarationSyntax classDecl,
            MethodDeclarationSyntax disposeMethodDecl)
        {
            var gcInvocation = GetGcInvocation(disposeMethodDecl);
            var newDisposeNode = disposeMethodDecl;
            if (gcInvocation is not null)
            {
                newDisposeNode = newDisposeNode.RemoveNode(gcInvocation, SyntaxRemoveOptions.KeepTrailingTrivia);
            }

            var leadingBlockTrivia = GetLeadingBodyTrivia(disposeMethodDecl);

            var newProtectedDisposeNode =
                CreateProtectedDisposeMethod(newDisposeNode!.Body, leadingBlockTrivia);

            newDisposeNode = newDisposeNode.WithBody(CreateDisposeMethodBlock())
                .WithTrailingTrivia(TriviaList(LineFeed));

            var members = classDecl.Members;

            members = members.Insert(0, CreateDisposeField());
            members = members.Add(newProtectedDisposeNode);

            var newClass = classDecl.WithMembers(members);

            var oldDisposeMethodDecl = newClass.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .First(m => m.Identifier.ToString().Equals("Dispose"));

            newClass = newClass.ReplaceNode(oldDisposeMethodDecl, newDisposeNode);

            return newClass;
        }
    }
}
