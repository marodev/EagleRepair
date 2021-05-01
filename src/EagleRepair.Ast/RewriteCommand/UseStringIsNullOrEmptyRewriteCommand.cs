using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.RewriteCommand
{
    public class UseStringIsNullOrEmptyRewriteCommand : AbstractRewriteCommand
    {
        public UseStringIsNullOrEmptyRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var op = node.OperatorToken.ToString();

            if (!op.Equals("&&") && !op.Equals("||"))
            {
                return base.VisitBinaryExpression(node);
            }
            
            // we are looking for a binary expr that has 2 children, which are as well binary expressions
            if (node.Left is not BinaryExpressionSyntax subLeftBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }
            
            if (node.Right is not BinaryExpressionSyntax subRightBinaryExpr)
            {
                return base.VisitBinaryExpression(node);
            }

            // check if first (left) expression is s != null
            if (subLeftBinaryExpr.Left is not IdentifierNameSyntax leftIdentifierName)
            {
                return base.VisitBinaryExpression(node);
            }
            
            if (!subLeftBinaryExpr.OperatorToken.ValueText.Equals("!="))
            {
                return base.VisitBinaryExpression(node);
            }
            
            if (subLeftBinaryExpr.Right is not LiteralExpressionSyntax nullLiteralExpr)
            {
                return base.VisitBinaryExpression(node);
            }
            
            if (!nullLiteralExpr.Token.ToString().Equals("null"))
            {
                return base.VisitBinaryExpression(node);
            }
            

            var rightIdentifier = subRightBinaryExpr.Left;
            IdentifierNameSyntax rightIdentifierName;
            
            switch (rightIdentifier)
            {
                case IdentifierNameSyntax identifierName:
                    {
                        // check if s != ""
                        if (!"!=".Equals(subRightBinaryExpr.OperatorToken.ToString()))
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        if (subRightBinaryExpr.Right is not LiteralExpressionSyntax literalExpr)
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        if (!literalExpr.Token.ValueText.Equals(""))
                        {
                            return base.VisitBinaryExpression(node); 
                        }
                        
                        rightIdentifierName = identifierName;
                        break;
                    }
                case MemberAccessExpressionSyntax memberAccess when !memberAccess.Name.ToString().Equals("Length"):
                    return base.VisitBinaryExpression(node);
                case MemberAccessExpressionSyntax memberAccess:
                    {
                        rightIdentifierName = memberAccess.Expression as IdentifierNameSyntax;

                        if (rightIdentifierName is null)
                        {
                            return base.VisitBinaryExpression(node);
                        }

                        break;
                    }
                default:
                    return base.VisitBinaryExpression(node);
            }

            if (!leftIdentifierName.ToString().Equals(rightIdentifierName.ToString()))
            {
                return base.VisitBinaryExpression(node);
            }
            
            var leftSymbol = _semanticModel.GetSymbolInfo(leftIdentifierName).Symbol?.ToString();
            var rightSymbol = _semanticModel.GetSymbolInfo(rightIdentifierName).Symbol?.ToString();

            // must be of type string, otherwise string.IsNullOrEmpty(..) can't be used.
            if (!"string".Equals(leftSymbol) || !leftSymbol.Equals(rightSymbol))
            {
                return base.VisitBinaryExpression(node);
            }

            var newNode = InjectUtils.CreateIsNotNullOrEmpty(leftIdentifierName.ToString());

            // keep original space after node
            newNode = newNode.WithTrailingTrivia(node.GetTrailingTrivia());
            return newNode;
        }
    }
}
