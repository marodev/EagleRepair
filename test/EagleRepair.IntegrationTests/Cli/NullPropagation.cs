using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.NullPropagation;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class NullPropagation
    {
        [Theory]
        [MemberData(nameof(SimpleNullPropagationDataProvider.TestCases), MemberType = typeof(SimpleNullPropagationDataProvider))]
        public async Task IfNotNull_InvokeMethod_ReturnsNullPropagation(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
