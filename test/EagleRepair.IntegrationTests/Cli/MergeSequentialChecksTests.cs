using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class MergeSequentialChecksTests
    {
        [Theory]
        [MemberData(nameof(MergeSequentialChecksNotNullDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNotNullDataProvider))]
        public async Task IfNotNull_AccessMember_ReturnsNullConditional(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksPatternMatchingDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksPatternMatchingDataProvider))]
        public async Task IfNotNull_AccessMemberPatternMatching_ReturnsConditionalPatternMatching(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksNullOrNullDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNullOrNullDataProvider))]
        public async Task IfNull_OrMemberAccessNull_ReturnsConditionalPatternMatching(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksNullOrOfTypeDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNullOrOfTypeDataProvider))]
        public async Task IfNull_OrMemberAccessIsType_ReturnsConditionalPatternMatching(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksNullOrSmallerDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNullOrSmallerDataProvider))]
        public async Task IfNull_OrMemberAccessIsSmaller_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksNotNullEqualsDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNotNullEqualsDataProvider))]
        public async Task IfNotNull_AndMemberAccessEquals_ReturnsNullConditionalAccess(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksNotNullWithNullableDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNotNullWithNullableDataProvider))]
        public async Task IfNotNull_AccessNullableType_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(MergeSequentialChecksNotEqualNegativeDataProvider.TestCases),
            MemberType = typeof(MergeSequentialChecksNotEqualNegativeDataProvider))]
        public async Task IfNotValue_AndEqualsValue_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }
    }
}
