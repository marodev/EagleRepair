using System.Linq;
using EagleRepair.Ast.Extensions;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using EagleRepair.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class TypeCheckAndCastRewriterR5 : AbstractRewriter
    {
        private readonly ITriviaService _triviaService;

        public TypeCheckAndCastRewriterR5(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService, ITriviaService triviaService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
            _triviaService = triviaService;
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
                parentExpr is not EqualsValueClauseSyntax &&
                parentExpr is not ArgumentSyntax &&
                parentExpr is not AssignmentExpressionSyntax)
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
            switch (parentExpr.Parent)
            {
                case ExpressionStatementSyntax expressionStatementSyntax:

                    var patVariableName = GetPatternVariableName(targetCastExpr);

                    if (string.IsNullOrEmpty(patVariableName))
                    {
                        return base.VisitIfStatement(node);
                    }

                    var isPatExpr = RewriteService.CreateIsPattern(left, targetCastExpr.Type,
                        patVariableName);

                    var newIdentifier = RewriteService.CreateIdentifier(patVariableName);
                    var newIfNodeWithWithAssignmentAndNoCast = node.ReplaceNode(targetCastExpr, newIdentifier);

                    newIfNodeWithWithAssignmentAndNoCast =
                        newIfNodeWithWithAssignmentAndNoCast.ReplaceNode(newIfNodeWithWithAssignmentAndNoCast.Condition,
                            isPatExpr);

                    ifStatementSyntax = newIfNodeWithWithAssignmentAndNoCast;

                    break;
                case ArgumentListSyntax:
                    var ptVariableName = GetPatternVariableName(targetCastExpr);

                    if (string.IsNullOrEmpty(ptVariableName))
                    {
                        return base.VisitIfStatement(node);
                    }

                    var isPatternExpr = RewriteService.CreateIsPattern(left, targetCastExpr.Type,
                        ptVariableName);

                    var newArgument = RewriteService.CreateArgument(ptVariableName);

                    var newIfNodeWithWithArgumentInsteadOfCast = node.ReplaceNode(parentExpr, newArgument);

                    ifStatementSyntax =
                        newIfNodeWithWithArgumentInsteadOfCast.ReplaceNode(
                            newIfNodeWithWithArgumentInsteadOfCast.Condition, isPatternExpr);
                    break;
                case MemberAccessExpressionSyntax memberAccessExpr:
                    {
                        var targetMethodName = memberAccessExpr.Name.Identifier.ValueText;
                        var patternVariableName = GetPatternVariableName(targetCastExpr);

                        if (string.IsNullOrEmpty(patternVariableName))
                        {
                            return base.VisitIfStatement(node);
                        }

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
                        // but keep any comments
                        // e.g., var str = (string) o;
                        var leadingTriviaToKeep = _triviaService.ExtractTriviaToKeep(localDecl.GetLeadingTrivia());
                        var trailingTriviaToKeep = _triviaService.ExtractTriviaToKeep(localDecl.GetTrailingTrivia());
                        var localDeclAnnotation = new SyntaxAnnotation("LocalDeclarationAnnotation");

                        var newLocalDecl = localDecl
                            .WithLeadingTrivia(leadingTriviaToKeep)
                            .WithTrailingTrivia(trailingTriviaToKeep)
                            .WithAdditionalAnnotations(localDeclAnnotation);

                        var newIfNode = node.ReplaceNode(localDecl, newLocalDecl);
                        var localDeclToRemove = newIfNode.GetAnnotatedNodes(localDeclAnnotation).First();
                        newIfNode = newIfNode.RemoveNode(localDeclToRemove, SyntaxRemoveOptions.KeepExteriorTrivia);

                        var firstBlockStatement = ((BlockSyntax)newIfNode?.Statement)?.Statements.FirstOrDefault();

                        if (firstBlockStatement is not null)
                        {
                            var leadingTriviaOfFirstIfStatement = firstBlockStatement.GetLeadingTrivia();
                            // we need to adjust the newline inside the block
                            // first child should not have a trailing EndOfLineTrivia
                            // if ( ... ) {
                            //                   --> remove trailing EndOfLineTrivia
                            //   try ( ...)      --> first child inside block statement
                            if (leadingTriviaOfFirstIfStatement.FirstOrDefault().IsKind(SyntaxKind.EndOfLineTrivia))
                            {
                                newIfNode = newIfNode.ReplaceNode(firstBlockStatement,
                                    firstBlockStatement.WithLeadingTrivia(leadingTriviaOfFirstIfStatement.Skip(1)
                                        .ToSyntaxTriviaList()));
                            }
                        }

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
                Text = message,
                SonarQubeId = SonarQubeRule.S3247.ToString(),
                ReSharperId = ReSharperRule.MergeCastWithTypeCheck.ToString()
            });

            // visit children of newIfNode
            return base.VisitIfStatement(ifStatementSyntax);
        }

        private static string ParseGenericName(GenericNameSyntax genericNameSyntax)
        {
            if (genericNameSyntax == null)
            {
                return null;
            }

            var patternVariableName = genericNameSyntax.TypeArgumentList.Arguments
                .FirstOrDefault()?.GetText().ToString();

            if (string.IsNullOrEmpty(patternVariableName))
            {
                return null;
            }

            patternVariableName += "s";

            return patternVariableName;
        }

        private string GetPatternVariableName(CastExpressionSyntax targetCastExpr)
        {
            string patternVariableName;
            if (targetCastExpr.Type.IsKind(SyntaxKind.ArrayType))
            {
                patternVariableName = targetCastExpr.Type.ToString();
                patternVariableName = patternVariableName.Split("[").FirstOrDefault();

                if (string.IsNullOrEmpty(patternVariableName))
                {
                    return null;
                }

                patternVariableName += "s";
            }
            else if (targetCastExpr.Type is QualifiedNameSyntax qfn &&
                     qfn.Right.IsKind(SyntaxKind.GenericName))
            {
                var rightQualifiedName = qfn.Right;
                patternVariableName = ParseGenericName(rightQualifiedName as GenericNameSyntax);
            }
            else if (targetCastExpr.Type.IsKind(SyntaxKind.GenericName))
            {
                patternVariableName = ParseGenericName(targetCastExpr.Type as GenericNameSyntax);
            }
            else
            {
                patternVariableName = targetCastExpr.Type.ToString().FirstCharToLowerCase();

                switch (patternVariableName)
                {
                    case "double":
                    case "int16":
                    case "int32":
                    case "int64":
                    case "decimal":
                    case "float":
                        patternVariableName = patternVariableName[0].ToString().ToLower();
                        break;
                    case "sbyte":
                    case "ushort":
                    case "uint":
                    case "ulong":
                    case "nuint":
                    case "short":
                        patternVariableName = patternVariableName[0].ToString().ToLower() +
                                              patternVariableName[1].ToString().ToLower();
                        break;
                    default:
                        // take first lowercase character if built-in type
                        patternVariableName = targetCastExpr.Type.Kind() == SyntaxKind.PredefinedType
                            ? targetCastExpr.Type.ToString()[0].ToString().ToLower()
                            : targetCastExpr.Type.ToString().FirstCharToLowerCase();
                        break;
                }
            }

            if (patternVariableName.Contains("."))
            {
                patternVariableName = patternVariableName.Split(".").Last().FirstCharToLowerCase();
            }

            if (TypeService.ContainsReservedKeyword(patternVariableName))
            {
                patternVariableName = $"@{patternVariableName}";
            }

            return patternVariableName;
        }
    }
}
