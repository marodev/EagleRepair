using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.NullChecksShouldNotBeUsedWithIs;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class NullChecksShouldNotBeUsedWithIsTests
    {
        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsAndDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsAndDataProvider))]
        public async Task UseNotNullCheckAndIs_ReturnsIs(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsOrIsNotDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsOrIsNotDataProvider))]
        public async Task UseNullCheckOrNotIs_ReturnsNotIs(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsCustomTypeDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsCustomTypeDataProvider))]
        public async Task UseNullCheck_AndCustomType_ReturnsIs(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsOrIsDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsOrIsDataProvider))]
        public async Task UseNullCheck_OrIs_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedNegativeDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedNegativeDataProvider))]
        public async Task UseNotNullCheck_AndIsNot_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsMemberAccessDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsMemberAccessDataProvider))]
        public async Task UseNotNullCheckProperty_AndIs_ReturnsIs(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsParenthesesDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsParenthesesDataProvider))]
        public async Task UseNullOrNotPattern_ReturnsIsNot(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
