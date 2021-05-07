using System.Collections.Generic;
using System.Linq;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EagleRepair.Ast.Rewriter
{
    public class UseNullPropagationRewriter : AbstractRewriter
    {
        public UseNullPropagationRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(changeTracker,
            typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var ifStatements = node.DescendantNodes()
                .OfType<IfStatementSyntax>()
                .Where(n => n.Else is null &&
                            n.Condition is BinaryExpressionSyntax &&
                            n.Statement is BlockSyntax).ToList();

            if (!ifStatements.Any())
            {
                return base.VisitMethodDeclaration(node);
            }

            var nodesToUpdate = new Dictionary<IfStatementSyntax, SyntaxNode>();
            foreach (var ifStatementSyntax in ifStatements)
            {
                var nullPropagation = TryNullPropagation(ifStatementSyntax);

                if (nullPropagation is null)
                {
                    continue;
                }

                nullPropagation = nullPropagation.WithLeadingTrivia(ifStatementSyntax.GetLeadingTrivia());
                nullPropagation = nullPropagation.WithTrailingTrivia(ifStatementSyntax.GetTrailingTrivia());

                nodesToUpdate.Add(ifStatementSyntax, nullPropagation);
            }

            if (!nodesToUpdate.Any())
            {
                return base.VisitMethodDeclaration(node);
            }

            var newMethodNode = node.ReplaceNodes(nodesToUpdate.Keys.AsEnumerable(), (n1, n2) => nodesToUpdate[n1]);

            foreach (var nodeToUpdate in nodesToUpdate)
            {
                var lineNumber = $"{DisplayService.GetLineNumber(nodeToUpdate.Key)}";
                var message = ReSharper.UseNullPropagationMessage;
                ChangeTracker.Add(new Message { Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message});
            }

            return base.VisitMethodDeclaration(newMethodNode);
        }

        private SyntaxNode TryNullPropagation(IfStatementSyntax node)
        {
            var binaryExpr = (BinaryExpressionSyntax)node.Condition;
            var block = (BlockSyntax)node.Statement;

            var statements = block.Statements;

            if (statements.Count != 1)
            {
                return null;
            }

            var statement = statements.Single();

            if (statement is not ExpressionStatementSyntax expressionStatement)
            {
                return null;
            }

            if (expressionStatement.Expression is not InvocationExpressionSyntax invocationExpr)
            {
                return null;
            }

            var left = binaryExpr.Left;

            if (left is not IdentifierNameSyntax identifierName)
            {
                return null;
            }

            if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return null;
            }

            if (memberAccessExpr.Expression is not IdentifierNameSyntax methodIdentifierName)
            {
                return null;
            }

            var variableName = methodIdentifierName.Identifier.ToString();

            var variableNameInCondition = identifierName.Identifier.ToString();
            if (!variableNameInCondition.Equals(variableName))
            {
                // if (s != null) { m.Foo() }
                return null;
            }

            var argsPassedToMethod = invocationExpr.ArgumentList;
            var methodName = memberAccessExpr.Name.Identifier.ToString();

            var newNullPropagationNode =
                RewriteService.CreateNullPropagation(variableName, methodName, argsPassedToMethod);

            return newNullPropagationNode;
        }
    }
}
