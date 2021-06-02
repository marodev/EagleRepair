using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class TypeCheckAndCastTests
    {
        [Theory]
        [MemberData(nameof(TypeCheckAndCastSimpleDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastSimpleDataProvider))]
        public async Task UseTypeCheck_IfAndWhileStatement_ReturnsPatternExpr(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastToNullDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastToNullDataProvider))]
        public async Task UseTypeCheck_ChecksToNull_ReturnsNotRefactored(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastTriviaDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastTriviaDataProvider))]
        public async Task UseTypeCheck_InBinaryExpr_ReturnsPatternExprWithTrivia(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastInMethodDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastInMethodDataProvider))]
        public async Task UseTypeCheck_InMethodCast_ReturnsPatternExprWithoutCast(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastCommentDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastCommentDataProvider))]
        public async Task UseTypeCheck_InMethodCastWithLeadingTrivia_ReturnsPatternExprWithoutCastAndLeadingTrivia(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastNewLineDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastNewLineDataProvider))]
        public async Task UseTypeCheck_NextStatementHasLeadingWhiteSpace_ReturnsPatternExprWithoutCastAndLeadingTrivia(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastArrayDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastArrayDataProvider))]
        public async Task UseTypeCheck_typeIsArray_ReturnsPatternExprWithArrayName(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastDoubleDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastDoubleDataProvider))]
        public async Task UseTypeCheck_typeIsDouble_ReturnsPatternExpr(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastIsPatternAndCastDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastIsPatternAndCastDataProvider))]
        public async Task UseTypeCheck_AndCast_ReturnsPatternExprWithoutCast(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastDecimalDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastDecimalDataProvider))]
        public async Task UseTypeCheck_AndCastWithDouble_ReturnsPatternExprWithoutCast(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastReservedKeywordDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastReservedKeywordDataProvider))]
        public async Task UseTypeCheck_WithReservedKeyword_ReturnsPatternExprAndAtChar(
            string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
