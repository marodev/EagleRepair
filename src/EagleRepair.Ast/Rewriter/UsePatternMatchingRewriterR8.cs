using System.Collections.Generic;
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
    public class UsePatternMatchingRewriterR8 : AbstractRewriter
    {
        private readonly ITriviaService _triviaService;

        public UsePatternMatchingRewriterR8(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService, ITriviaService triviaService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
            _triviaService = triviaService;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var localDeclarationStatements = node.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().ToList();
            var binaryExprStmts = node.DescendantNodes().OfType<BinaryExpressionSyntax>().ToList();

            if (!localDeclarationStatements.Any() || !binaryExprStmts.Any())
            {
                return base.VisitMethodDeclaration(node);
            }

            var oldNewNodeDict = new Dictionary<CSharpSyntaxNode, CSharpSyntaxNode>();
            var messagesToReportDict = new Dictionary<CSharpSyntaxNode, CSharpSyntaxNode>();
            var childOfIfCondition = new SyntaxAnnotation("ChildOfIfCondition");
            var childOfChildOfIfCondition = new SyntaxAnnotation("ChildOfChildOfIfCondition");

            var removeAsExpression = new SyntaxAnnotation("removeAsExpression");
            foreach (var localDeclaration in localDeclarationStatements)
            {
                var declaration = localDeclaration.Declaration.Variables.FirstOrDefault();

                if (declaration?.Initializer is null)
                {
                    continue;
                }

                var identifierName = declaration.Identifier.ToString();
                var expression = declaration.Initializer.Value;

                if (expression is not BinaryExpressionSyntax binaryExpr)
                {
                    continue;
                }

                var right = binaryExpr.Right.ToString();
                var op = binaryExpr.OperatorToken.ToString();

                if (!"as".Equals(op))
                {
                    continue;
                }

                var binaryExpressionsToReplace = FindBinaryExprToReplace(identifierName, binaryExprStmts);

                if (!binaryExpressionsToReplace.Any())
                {
                    continue;
                }

                foreach (var binaryExprToReplace in binaryExpressionsToReplace)
                {
                    TypeSyntax s;
                    if (binaryExpr.Right is PredefinedTypeSyntax predefinedTypeSyntax)
                    {
                        s = predefinedTypeSyntax;
                    }
                    else
                    {
                        s = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(right));
                    }

                    var newConditionExpr = RewriteService
                        .CreateIsPattern(binaryExpr.Left, s, identifierName, binaryExprToReplace);

                    if (!oldNewNodeDict.ContainsKey(localDeclaration))
                    {
                        // mark node to be removed with an annotation
                        oldNewNodeDict.Add(localDeclaration,
                            localDeclaration.WithAdditionalAnnotations(removeAsExpression));
                    }

                    if (oldNewNodeDict.ContainsKey(binaryExprToReplace))
                    {
                        continue;
                    }

                    if (binaryExprToReplace.Parent is IfStatementSyntax)
                    {
                        newConditionExpr = newConditionExpr.WithAdditionalAnnotations(childOfIfCondition);
                    }

                    if (binaryExprToReplace.Parent?.Parent is IfStatementSyntax)
                    {
                        // TODO: delte me
                        newConditionExpr = newConditionExpr.WithAdditionalAnnotations(childOfChildOfIfCondition);
                        // oldNewNodeDict.Add(binaryExprToReplace, newConditionExpr);
                    }

                    oldNewNodeDict.Add(binaryExprToReplace, newConditionExpr);
                    messagesToReportDict.Add(binaryExprToReplace, newConditionExpr);
                }
            }

            if (!oldNewNodeDict.Any())
            {
                // there are no changes
                return base.VisitMethodDeclaration(node);
            }

            var newMethod = node.ReplaceNodes(oldNewNodeDict.Keys.AsEnumerable(),
                (n1, n2) => oldNewNodeDict[n1]);

            var nodesToBeRemoved = newMethod.GetAnnotatedNodes(removeAsExpression);

            foreach (var _ in nodesToBeRemoved)
            {
                // TODO: duplicated code in TypeCheckAndCast --> Refactor
                // create a copy of the node to be replaced to keep important trivias (comments)
                var nodeToBeRemoved = newMethod!.GetAnnotatedNodes(removeAsExpression).First();
                var leadingTriviaToKeep = _triviaService.ExtractTriviaToKeep(nodeToBeRemoved.GetLeadingTrivia());
                var trailingTriviaToKeep = _triviaService.ExtractTriviaToKeep(nodeToBeRemoved.GetTrailingTrivia());
                var nodeToBeRemovedAnnotation = new SyntaxAnnotation("nodeToBeRemovedAnnotation");
                var newNodeToBeRemoved = nodeToBeRemoved
                    .WithLeadingTrivia(leadingTriviaToKeep)
                    .WithTrailingTrivia(trailingTriviaToKeep)
                    .WithAdditionalAnnotations(nodeToBeRemovedAnnotation);

                newMethod = newMethod.ReplaceNode(nodeToBeRemoved, newNodeToBeRemoved);
                newNodeToBeRemoved = newMethod.GetAnnotatedNodes(nodeToBeRemovedAnnotation).First();

                newMethod = newMethod.RemoveNode(newNodeToBeRemoved, SyntaxRemoveOptions.KeepExteriorTrivia);
            }

            // deals with whitespace
            var ifStatementChildren = newMethod!.GetAnnotatedNodes(childOfIfCondition);
            var ifStatementsToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            ifStatementsToReplace =
                ifStatementsToReplace.Merge(AddLeadingLineFeedToIfStatements(ifStatementChildren.ToList()));

            var childrenOfChildrenOfIfCondition = newMethod.GetAnnotatedNodes(childOfChildOfIfCondition);
            var potentialChildrenOfIfStatement = childrenOfChildrenOfIfCondition
                .Where(n => n.Parent != null)
                .Select(n => n.Parent).ToList();

            ifStatementsToReplace =
                ifStatementsToReplace.Merge(AddLeadingLineFeedToIfStatements(potentialChildrenOfIfStatement));

            newMethod = newMethod.ReplaceNodes(ifStatementsToReplace.Keys.AsEnumerable(),
                (n1, n2) => ifStatementsToReplace[n1]);

            // report fixes
            foreach (var nodeToUpdate in messagesToReportDict)
            {
                var lineNumber = $"{DisplayService.GetLineNumber(nodeToUpdate.Key)}";
                var message = ReSharper.UsePatternMatchingMessage;
                ChangeTracker.Stage(new Message
                {
                    RuleId = nameof(Rule.R8),
                    LineNr = lineNumber,
                    FilePath = FilePath,
                    ProjectName = ProjectName,
                    Text = message
                });
            }

            return base.VisitMethodDeclaration(newMethod);
        }

        private static IDictionary<SyntaxNode, SyntaxNode> AddLeadingLineFeedToIfStatements(
            IEnumerable<SyntaxNode> ifStatementChildren)
        {
            var ifStatementsToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var ifStatementChild in ifStatementChildren)
            {
                var firstChild = ifStatementChild.Parent?.Parent?.DescendantNodes().FirstOrDefault();

                if (firstChild == null)
                {
                    continue;
                }

                // check if ifStatementChild is first child
                if (firstChild.IsEquivalentTo(ifStatementChild.Parent))
                {
                    var firstChildLeadingTrivia = firstChild.GetLeadingTrivia();
                    var position = -1;
                    for (var i = 0; i < firstChildLeadingTrivia.Count; i++)
                    {
                        if (IsNewLine(firstChildLeadingTrivia[i]))
                        {
                            position = i;
                            break;
                        }

                        var nextTriviaPos = i + 1;
                        if (nextTriviaPos >= firstChildLeadingTrivia.Count)
                        {
                            // already reached the end
                            break;
                        }

                        // check one token ahead. The goal is to detect comments (//, /* ... */) that we don't want to remove
                        if (!string.IsNullOrWhiteSpace(firstChildLeadingTrivia[nextTriviaPos].ToString()) &&
                            !IsNewLine(firstChildLeadingTrivia[nextTriviaPos]))
                        {
                            // the whitespace probably belongs to the following comment.
                            position = -1;
                            break;
                        }

                        position++;
                    }

                    if (position >= 0)
                    {
                        // we have to remove the leading trivia since the node is now the first child
                        // before:
                        // var s = obj as string  --> we removed this node
                        //                        --> new line still present, remove this
                        // if { ... }             --> node to keep

                        var newFirstChildTrivia = ifStatementChild.Parent.GetLeadingTrivia();
                        var newTrivia = newFirstChildTrivia.Skip(position + 1).ToSyntaxTriviaList();

                        ifStatementsToReplace.Add(ifStatementChild.Parent,
                            ifStatementChild.Parent.WithLeadingTrivia(newTrivia));
                    }

                    continue;
                }

                // node is not first child, we want a new line just before the if condition
                // }            --> some other block
                //              --> target new line to insert
                // if ( ... )   --> our if condition
                var leadingTriviaList = ifStatementChild.Parent.GetLeadingTrivia();
                var leadingTrivia = leadingTriviaList.FirstOrDefault();

                if (IsNewLine(leadingTrivia))
                {
                    // already contains a new line, don't add an additional one
                    continue;
                }

                var existingTrivia = leadingTriviaList.Insert(0, SyntaxFactory.LineFeed);
                ifStatementsToReplace.Add(ifStatementChild.Parent,
                    ifStatementChild.Parent.WithLeadingTrivia(existingTrivia));
            }

            return ifStatementsToReplace;
        }

        /// <summary>
        ///     \n or \r\n
        /// </summary>
        /// <param name="trivia"></param>
        /// <returns></returns>
        private static bool IsNewLine(SyntaxTrivia trivia)
        {
            var triviaStr = trivia.ToString();
            return triviaStr.Equals(SyntaxFactory.LineFeed.ToString()) ||
                   triviaStr.Equals(SyntaxFactory.CarriageReturnLineFeed.ToString());
        }

        private static List<BinaryExpressionSyntax> FindBinaryExprToReplace(string variableName,
            IEnumerable<BinaryExpressionSyntax> binaryExprStmts)
        {
            var found = new List<BinaryExpressionSyntax>();

            foreach (var binaryExpr in binaryExprStmts)
            {
                var left = binaryExpr.Left.ToString();
                var op = binaryExpr.OperatorToken.ValueText;
                var right = binaryExpr.Right.ToString();

                if (!"!=".Equals(op) || !left.Equals(variableName) || !right.Equals("null"))
                {
                    continue;
                }

                found.Add(binaryExpr);
            }

            return found;
        }
    }
}
