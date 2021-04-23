using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ast.SyntaxRewriter
{
    public class DummySyntaxRewriter : CSharpSyntaxRewriter
    {
        /// Visited for all AttributeListSyntax nodes
        /// The method replaces all PreviousAttribute attributes annotating a method by ReplacementAttribute attributes
        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            // If the parent is a MethodDeclaration (= the attribute annotes a method)
            if (node.Parent is MethodDeclarationSyntax &&
                // and if the attribute name is PreviousAttribute
                node.Attributes.Any(
                    currentAttribute => currentAttribute.Name.GetText().ToString() == "PreviousAttribute"))
            {
                // Return an alternate node that is injected instead of the current node
                return SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("ReplacementAttribute"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList(new[]
                                {
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(@"Sample"))
                                    )
                                })))));
            }

            // Otherwise the node is left untouched
            return base.VisitAttributeList(node);
        }
    }

}