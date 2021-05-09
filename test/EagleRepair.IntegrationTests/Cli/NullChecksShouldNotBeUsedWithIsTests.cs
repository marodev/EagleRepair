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
        [MemberData(nameof(NullChecksShouldNotBeUsedWithIsOrDataProvider.TestCases),
            MemberType = typeof(NullChecksShouldNotBeUsedWithIsOrDataProvider))]
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
    }
}
