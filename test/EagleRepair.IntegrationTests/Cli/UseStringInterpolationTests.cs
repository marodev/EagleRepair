using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class UseStringInterpolationTests
    {
        [Theory]
        [MemberData(nameof(UseStringInterpolationDataProvider.TestCases),
            MemberType = typeof(UseStringInterpolationDataProvider))]
        public async Task UseStringFormat_ReturnsStringInterpolation(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
