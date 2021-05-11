using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class MergeSequentialChecksRewriterR2 : AbstractRewriter
    {
        public MergeSequentialChecksRewriterR2(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
        }

        private ExpressionSyntax FixRightBinaryExpr(string leftExprVariableName,
            BinaryExpressionSyntax binaryExpr)
        {
            if (binaryExpr.Left is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return null;
            }

            if (memberAccessExpr.Expression is not IdentifierNameSyntax identifierName)
            {
                return null;
            }

            var variableName = identifierName.ToString();
            if (!leftExprVariableName.Equals(variableName))
            {
                return null;
            }

            var invokedMemberName = memberAccessExpr.Name.ToString();

            var op = binaryExpr.OperatorToken;
            switch (binaryExpr.Right)
            {
                case LiteralExpressionSyntax literalExpr when !literalExpr.ToString().Equals("null"):
                    return null;
                case LiteralExpressionSyntax:
                    return RewriteService.CreateNullPatternExprWithConditionalMemberAccess(variableName,
                        op.ValueText, invokedMemberName);
                case IdentifierNameSyntax rightIdentifierName:
                    {
                        var targetType = rightIdentifierName.Identifier.ToString();

                        // s.X is Foo
                        if (binaryExpr.OperatorToken.IsKind(SyntaxKind.IsKeyword))
                        {
                            return RewriteService.CreateIsTypePatternExprWithConditionalMemberAccess(variableName,
                                invokedMemberName, op.ValueText, targetType);
                        }

                        return RewriteService.CreateConditionalBinaryExpr(variableName, invokedMemberName, op,
                            binaryExpr.Right);
                    }

                default:
                    return RewriteService.CreateConditionalBinaryExpr(variableName, invokedMemberName, op,
                        binaryExpr.Right);
            }
        }

        private ExpressionSyntax FixRightIsPatternExpr(string leftExprVariableName,
            IsPatternExpressionSyntax isPatternExpr)
        {
            if (isPatternExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return null;
            }

            PatternSyntax declarationOrConstNullPattern;
            switch (isPatternExpr.Pattern)
            {
                case DeclarationPatternSyntax declarationPattern:
                    declarationOrConstNullPattern = declarationPattern;
                    break;
                case ConstantPatternSyntax constantPattern:
                    declarationOrConstNullPattern = constantPattern;
                    break;
                default:
                    return null;
            }

            if (memberAccessExpr.Name is not IdentifierNameSyntax invokedMemberName)
            {
                return null;
            }

            if (memberAccessExpr.Expression is not IdentifierNameSyntax invokedMemberIdentifierName)
            {
                return null;
            }

            var variableName = invokedMemberIdentifierName.Identifier.ToString();
            if (!leftExprVariableName.Equals(variableName))
            {
                return null;
            }

            var opKeyword = isPatternExpr.IsKeyword.ToString();

            IsPatternExpressionSyntax newNode = null;
            switch (declarationOrConstNullPattern)
            {
                case DeclarationPatternSyntax declPattern:
                    newNode = RewriteService.CreateIsPatternExprWithConditionalMemberAccessAndDeclaration(variableName,
                        opKeyword,
                        invokedMemberName.ToString(), declPattern.Type.ToString(),
                        declPattern.Designation.ToString()).NormalizeWhitespace();
                    break;
                case ConstantPatternSyntax constantPattern:
                    {
                        if (constantPattern.Expression is not LiteralExpressionSyntax literalExpr)
                        {
                            return null;
                        }

                        if (!literalExpr.ToString().Equals("null"))
                        {
                            return null;
                        }

                        newNode = RewriteService.CreateNullPatternExprWithConditionalMemberAccess(variableName,
                            opKeyword,
                            invokedMemberName.ToString());
                        break;
                    }
            }

            return newNode;
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var allBinaryExprCount = node.DescendantNodes().OfType<BinaryExpressionSyntax>().Count();

            if (allBinaryExprCount != 2 && node.Right is not IsPatternExpressionSyntax)
            {
                return base.VisitBinaryExpression(node);
            }

            if (node.Left is not BinaryExpressionSyntax leftBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            if (leftBinaryExpr.Left is not IdentifierNameSyntax leftLeftIdentifierName)
            {
                return base.VisitBinaryExpression(node);
            }

            var symbol = SemanticModel.GetSymbolInfo(leftLeftIdentifierName).Symbol;

            if (symbol is null)
            {
                return base.VisitBinaryExpression(node);
            }

            var typeSymbol = symbol switch
            {
                ILocalSymbol iLocalSymbol => iLocalSymbol.Type,
                IParameterSymbol iParameterSymbol => iParameterSymbol.Type,
                _ => null
            };

            if (typeSymbol is null || typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
            {
                // we can't use the conditional operator ? (e.g, s?.Value) for nullable types
                return base.VisitBinaryExpression(node);
            }

            if (leftBinaryExpr.OperatorToken.ToString().Equals("=="))
            {
                if (node.Right is not IsPatternExpressionSyntax)
                {
                    if (node.Right is BinaryExpressionSyntax rightBinaryExpr)
                    {
                        if (!rightBinaryExpr.OperatorToken.IsKind(SyntaxKind.IsKeyword))
                        {
                            // we can't fix something like if (s == null || s.Length != count)
                            return base.VisitBinaryExpression(node);
                        }
                    }
                }
            }

            var leftExprVariableName = leftLeftIdentifierName.ToString();
            var newNode = node.Right switch
            {
                BinaryExpressionSyntax rightBinaryExpr => FixRightBinaryExpr(leftExprVariableName, rightBinaryExpr),
                IsPatternExpressionSyntax isPatternExpr => FixRightIsPatternExpr(leftExprVariableName, isPatternExpr),
                _ => null
            };

            if (newNode is null)
            {
                return base.VisitBinaryExpression(node);
            }

            // keep original space
            newNode = newNode.WithTrailingTrivia(node.Right.GetTrailingTrivia());

            var lineNumber = $"{DisplayService.GetLineNumber(node)}";
            var message = ReSharper.MergeSequentialChecksUrl;
            ChangeTracker.Stage(new Message
            {
                RuleId = nameof(Rule.R2),
                LineNr = lineNumber,
                FilePath = FilePath,
                ProjectName = ProjectName,
                Text = message
            });

            return newNode;
        }
    }
}
