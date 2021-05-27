using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.UseStringIsNullOrEmpty;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class UseStringIsNullOrEmptyTests
    {
        [Theory]
        [MemberData(nameof(UseStringIsNullOrEmptySimpleDataProvider.TestCases),
            MemberType = typeof(UseStringIsNullOrEmptySimpleDataProvider))]
        public async Task UseNullCheckAndLength_ReturnsStringIsNullOrEmpty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UseStringIsNullOrEmptyEdgeCasesDataProvider.TestCases),
            MemberType = typeof(UseStringIsNullOrEmptyEdgeCasesDataProvider))]
        public async Task UseNullCheckAndEmpty_ReturnsStringIsNullOrEmpty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UseStringIsNullOrEmptyNegativeDataProvider.TestCases),
            MemberType = typeof(UseStringIsNullOrEmptyNegativeDataProvider))]
        public async Task UseNullCheckAndLengthGreaterAs_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(UseStringIsNullOrEmptyStringEmptyDataProvider.TestCases),
            MemberType = typeof(UseStringIsNullOrEmptyStringEmptyDataProvider))]
        public async Task UseStringEmpty_ReturnsStringIsNullOrEmpty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
