using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ast.SyntaxRewriter
{
    public class UseMethodAnySyntaxRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            if (node.Condition is not BinaryExpressionSyntax condition)
            {
                return base.VisitIfStatement(node);
            }

            var left = condition.Left;

            if (left is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return base.VisitIfStatement(node);
            }

            var targetMethodName = memberAccessExpressionSyntax.Name.Identifier.ValueText;

            if (!"Count".Equals(targetMethodName))
            {
                return base.VisitIfStatement(node);
            }
            
            var right = condition.Right;

            if (right is not LiteralExpressionSyntax literalExpressionSyntax)
            {
                return base.VisitIfStatement(node);
            }
            
            var valueText = literalExpressionSyntax.Token.Text;

            if (!valueText.Equals("0"))
            {
                return base.VisitIfStatement(node);
            }
            
            return TransformToInvocationExpression(node);
        }

        // public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        // {
        //     return base.VisitMemberAccessExpression(node);
        // }

        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {
            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return base.VisitMethodDeclaration(node);
        }

        // public override SyntaxTokenList VisitList(SyntaxTokenList list)
        // {
        //     return base.VisitList(list);
        // }
        //
        // public override SyntaxTriviaList VisitList(SyntaxTriviaList list)
        // {
        //     return base.VisitList(list);
        // }

        public override SyntaxNode? VisitAccessorList(AccessorListSyntax node)
        {
            return base.VisitAccessorList(node);
        }

        private static IfStatementSyntax TransformToInvocationExpression(IfStatementSyntax node)
        {
            var condition = (BinaryExpressionSyntax) node.Condition;
            var left = (MemberAccessExpressionSyntax) condition.Left;
            var name = (IdentifierNameSyntax) left.Expression;
            var identifier = name.Identifier;

            var collectionName = identifier.ValueText;

            var conditionFix = SyntaxFactory.InvocationExpression
                (
                    SyntaxFactory.MemberAccessExpression
                        (
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName(collectionName),
                            SyntaxFactory.IdentifierName("Any")
                        )
                        .WithOperatorToken
                        (
                            SyntaxFactory.Token(SyntaxKind.DotToken)
                        )
                )
                .WithArgumentList
                (
                    SyntaxFactory.ArgumentList()
                        .WithOpenParenToken
                        (
                            SyntaxFactory.Token(SyntaxKind.OpenParenToken)
                        )
                        .WithCloseParenToken
                        (
                            SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                        )
                );
            
            return node.WithCondition(conditionFix);
        }
    }
}