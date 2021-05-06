using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Services
{
    public interface IRewriteService
    {
        public CompilationUnitSyntax InjectUsingDirective(CompilationUnitSyntax compilation, string usingDirective);

        public BinaryExpressionSyntax ConnectBinaryExpr(BinaryExpressionSyntax root, SyntaxNode left, SyntaxNode right,
            string op);

        public MemberAccessExpressionSyntax CreateMemberAccess(string variable, string methodName);

        public InvocationExpressionSyntax CreateInvocation(string variable, string methodName,
            ArgumentListSyntax arguments = null);

        public IsPatternExpressionSyntax CreateIsPattern(string identifierName, string type, string designation);

        public InvocationExpressionSyntax CreateOfTypeT(string variable, string type);

        public ExpressionStatementSyntax CreateNullPropagation(string variableName, string methodName,
            ArgumentListSyntax arguments = null);

        public InterpolatedStringExpressionSyntax CreateInterpolatedString(
            SeparatedSyntaxList<ArgumentSyntax> allArguments);

        public PrefixUnaryExpressionSyntax CreateIsNotNullOrEmpty(string variableName);

        public ExpressionSyntax ConvertUnaryToIsNotPattern(PrefixUnaryExpressionSyntax unaryExpr);

        public ExpressionSyntax CreateConditionalAccess(string variableName, string memberName);

        public IsPatternExpressionSyntax CreateNullPatternExprWithConditionalMemberAccess(string variableName,
            string op, string memberName);

        public IsPatternExpressionSyntax CreateIsPatternExprWithConditionalMemberAccessAndDeclaration(
            string variableName, string op, string memberName, string targetTypeName, string declarationName);

        public IsPatternExpressionSyntax CreateIsTypePatternExprWithConditionalMemberAccess(string variableName,
            string memberName, string op, string targetTypeName);

        public SyntaxNode AddSealedKeyword(ClassDeclarationSyntax classDecl);

        public ClassDeclarationSyntax ModifyDisposeAndAddProtectedDispose(ClassDeclarationSyntax classDecl,
            MethodDeclarationSyntax disposeMethodDecl);
    }
}
