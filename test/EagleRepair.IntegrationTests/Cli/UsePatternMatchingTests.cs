using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class UsePatternMatchingTests
    {
        [Theory]
        [MemberData(nameof(UsePatternMatchingDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingDataProvider))]
        public async Task UseSuboptimalCode_ReturnsPatternMatching(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingNegativeDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingNegativeDataProvider))]
        public async Task UseDifferentVariableInNullCheck_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingComplexDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingComplexDataProvider))]
        public async Task UseMethodAccess_ExplicitlyConvertAs_ReturnsPatternMatching(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingMultipleAccessDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingMultipleAccessDataProvider))]
        public async Task UseAsType_VariableAccessedMultipleTimesNotInCondition_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false, true);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingMultiplePatternsDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingMultiplePatternsDataProvider))]
        public async Task UseAsType_MultipleAsTypesInIfElse_ReturnsPatternMatching(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingNotInConditionDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingNotInConditionDataProvider))]
        public async Task UseAsType_NotInCondition_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false, true);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingWhitespaceDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingWhitespaceDataProvider))]
        public async Task UseAsType_AfterwardWhitespace_ReturnsWhitespaceAndAsPattern(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingBinaryExprWhitespaceDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingBinaryExprWhitespaceDataProvider))]
        public async Task UseAsType_AfterwardBinaryExpr_ReturnsAsPatternAndWhitespace(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingMethodInvocationDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingMethodInvocationDataProvider))]
        public async Task UseAsType_WithMethodInvocation_ReturnsAsPatternFormatted(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingTriviaDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingTriviaDataProvider))]
        public async Task UseAsType_WithWhitespace_ReturnsAsPatternWithoutWhitespace(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
