using System.Linq;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class UseMethodAnyRewriteCommand : AbstractRewriteCommand
    {
        public UseMethodAnyRewriteCommand(IChangeTracker changeTracker) : base(changeTracker)
        {
        }



        public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
        {
            return base.VisitCompilationUnit(node);
        }

        // public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        // {
        //     if (node.Name.Identifier.ValueText.Equals("Count"))
        //     {
        //         Console.WriteLine("miau");
        //     } 
        //     return base.VisitMemberAccessExpression(node);
        // }

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            var memberAccessNodes = node.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            
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
            
            // InjectUsingDirective(node, "System.Linq");
            return TransformToInvocationExpression(node);
        }

        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            return base.VisitMemberAccessExpression(node);
        }

        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {
            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode? VisitUsingDirective(UsingDirectiveSyntax node)
        {
            return base.VisitUsingDirective(node);
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

            var location = identifier.GetLocation();

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


        private static void InjectUsingDirective(SyntaxNode statementSyntax, string usingDirective)
        {
            if (statementSyntax.Parent != null)
            {
                // find root
                InjectUsingDirective(statementSyntax.Parent, usingDirective);
                return;
            }

            if (statementSyntax is not CompilationUnitSyntax compilationUnitSyntax)
            {
                return;
            }
            
            var allUsings = compilationUnitSyntax.Usings;
            var insertPos = 0; // position to insert using directive
            foreach (var compareResult in allUsings
                .Select(usingDirectiveSyntax => usingDirectiveSyntax.Name.ToString())
                .Select(usingName => string.Compare(usingName, usingDirective)))
            {
                switch (compareResult)
                {
                    case 0: // using directive already exist
                        return;
                    case < 1:
                        insertPos++;
                        break;
                }
            }

            allUsings = allUsings.Insert(insertPos, CreateUsingDirective("System", "Linq"));
            compilationUnitSyntax = compilationUnitSyntax.WithUsings(allUsings);
        }

        private static UsingDirectiveSyntax CreateUsingDirective(string firstIdentifier, string secondIdentifier)
        {
            return SyntaxFactory.UsingDirective
                (
                    SyntaxFactory.QualifiedName
                        (
                            SyntaxFactory.IdentifierName(firstIdentifier),
                            SyntaxFactory.IdentifierName(secondIdentifier)
                        )
                        .WithDotToken
                        (
                            SyntaxFactory.Token(SyntaxKind.DotToken)
                        )
                )
                .WithUsingKeyword
                (
                    SyntaxFactory.Token
                    (
                        SyntaxFactory.TriviaList(),
                        SyntaxKind.UsingKeyword,
                        SyntaxFactory.TriviaList
                        (
                            SyntaxFactory.Space
                        )
                    )
                )
                .WithSemicolonToken
                (
                    SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken)
                );
        }
    }
}