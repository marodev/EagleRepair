using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.MultipleRules;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class MultipleRulesTest
    {
        [Theory]
        [MemberData(nameof(MultipleRulesDataProvider.TestCases),
            MemberType = typeof(MultipleRulesDataProvider))]
        public async Task GivenPatternMatchingAndStringIsNullOrEmpty_ReturnsBothFixes(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
