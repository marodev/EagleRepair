using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.Engine;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class EngineTests
    {
        [Theory]
        [MemberData(nameof(EngineShouldNotAcceptDataProvider.TestCases),
            MemberType = typeof(EngineShouldNotAcceptDataProvider))]
        public async Task CanBeRefactored_ButNotInGivenContext_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }
    }
}
