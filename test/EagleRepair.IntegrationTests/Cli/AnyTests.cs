using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.Any;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class AnyTests
    {
        [Theory]
        [MemberData(nameof(AnySimpleDataProvider.TestCases), MemberType = typeof(AnySimpleDataProvider))]
        public async Task UseCount_IfAndWhileStatement_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }


        [Theory]
        [MemberData(nameof(AnyNestedDataProvider.TestCases), MemberType = typeof(AnyNestedDataProvider))]
        public async Task UseCount_MultipleMethodCalls_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(AnyNegativeDataProvider.TestCases), MemberType = typeof(AnyNegativeDataProvider))]
        public async Task UseCount_NoIEnumerableImplemented_ReturnsCount(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(AnyIEnumerableDataProvider.TestCases), MemberType = typeof(AnyIEnumerableDataProvider))]
        public async Task UseCount_ImplementsIEnumerable_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(AnyMultipleBinaryExprDataProvider.TestCases),
            MemberType = typeof(AnyMultipleBinaryExprDataProvider))]
        public async Task UseCount_UsesMultipleBinaryExpr_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(AnyLoopDataProvider.TestCases),
            MemberType = typeof(AnyLoopDataProvider))]
        public async Task UseCount_UsesLoop_ReturnsCount(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(AnyNullableDataProvider.TestCases),
            MemberType = typeof(AnyNullableDataProvider))]
        public async Task UseCount_WithinConditionalAccess_ReturnsCount(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(AnyCountNotNullDataProvider.TestCases),
            MemberType = typeof(AnyCountNotNullDataProvider))]
        public async Task UseCount_NotZero_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(AnyMemberAccessDataProvider.TestCases),
            MemberType = typeof(AnyMemberAccessDataProvider))]
        public async Task UseCount_WithList_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }
    }
}
