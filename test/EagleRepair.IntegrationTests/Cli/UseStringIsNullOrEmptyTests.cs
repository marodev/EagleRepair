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
    }
}