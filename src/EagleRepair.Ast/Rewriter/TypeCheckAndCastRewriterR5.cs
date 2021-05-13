using System.Linq;
using EagleRepair.Ast.Extensions;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    // TODO: needs improvement, i.e., doesn't cover all cases
    public class TypeCheckAndCastRewriterR5 : AbstractRewriter
    {
        public TypeCheckAndCastRewriterR5(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            var condition = node.Condition;

            if (condition is not BinaryExpressionSyntax binaryExpr)
            {
                return base.VisitIfStatement(node);
            }

            var left = binaryExpr.Left;
            var op = binaryExpr.OperatorToken;
            var right = binaryExpr.Right;

            if (left is not IdentifierNameSyntax || !op.Text.Equals("is"))
            {
                return base.VisitIfStatement(node);
            }

            var variableName = left.TryGetInferredMemberName();

            if (string.IsNullOrEmpty(variableName))
            {
                return base.VisitIfStatement(node);
            }

            var castExpressions = node.Statement.DescendantNodes().OfType<CastExpressionSyntax>();

            var targetCastExpr = castExpressions
                .FirstOrDefault(c => c.Expression is IdentifierNameSyntax identifier &&
                                     variableName.Equals(identifier.TryGetInferredMemberName()));

            var parentExpr = targetCastExpr?.Parent;

            if (parentExpr is not ParenthesizedExpressionSyntax &&
                parentExpr is not EqualsValueClauseSyntax)
            {
                return base.VisitIfStatement(node);
            }

            var targetType = targetCastExpr.Type.ToString();

            if (!targetType.Equals(right.ToString()))
            {
                // node is casted to another type
                return base.VisitIfStatement(node);
            }

            IfStatementSyntax ifStatementSyntax;
            // TODO: we might have to cover more variations here
            switch (parentExpr.Parent)
            {
                case MemberAccessExpressionSyntax memberAccessExpr:
                    {
                        var targetMethodName = memberAccessExpr.Name.Identifier.ValueText;

                        // take first lowercase character if built-in type
                        var patternVariableName = targetCastExpr.Type.Kind() == SyntaxKind.PredefinedType
                            ? targetType[0].ToString().ToLower()
                            : targetType.FirstCharToLowerCase();

                        var newMethodInvocation =
                            RewriteService.CreateMemberAccess(patternVariableName, targetMethodName);
                        newMethodInvocation = newMethodInvocation.WithTriviaFrom(memberAccessExpr);

                        var newIfNode = node.ReplaceNode(memberAccessExpr, newMethodInvocation);

                        var patternExpr = RewriteService.CreateIsPattern(left, targetCastExpr.Type,
                            patternVariableName);

                        ifStatementSyntax = newIfNode.ReplaceNode(newIfNode.Condition, patternExpr);
                        break;
                    }
                case VariableDeclaratorSyntax:
                    {
                        var localDeclarationStatement = parentExpr.Parent?.Parent?.Parent;
                        if (localDeclarationStatement is not LocalDeclarationStatementSyntax localDecl)
                        {
                            return base.VisitIfStatement(node);
                        }

                        var patternVariableName = localDecl.Declaration.Variables
                            .FirstOrDefault()
                            ?.Identifier.ToString();

                        if (string.IsNullOrEmpty(patternVariableName))
                        {
                            return base.VisitIfStatement(node);
                        }

                        // remove assignment
                        // e.g., var str = (string) o;
                        var newIfNode = node.RemoveNode(localDecl, SyntaxRemoveOptions.KeepNoTrivia);

                        var patternExpr = RewriteService.CreateIsPattern(left, targetCastExpr.Type,
                            patternVariableName);

                        // replace type check with pattern expression
                        ifStatementSyntax = newIfNode!.ReplaceNode(newIfNode.Condition, patternExpr);
                        break;
                    }
                default:
                    return base.VisitIfStatement(node);
            }

            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.MergeCastWithTypeCheckMessage + " / " + SonarQube.RuleSpecification3247Message;
            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R5),
                LineNr = lineNumber,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = message
            });

            // visit children of newIfNode
            return base.VisitIfStatement(ifStatementSyntax);
        }
    }
}
