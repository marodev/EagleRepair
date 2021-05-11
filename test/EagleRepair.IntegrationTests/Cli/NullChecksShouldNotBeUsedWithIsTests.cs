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
    }
}
