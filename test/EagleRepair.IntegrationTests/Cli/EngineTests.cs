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

        [Theory]
        [MemberData(nameof(EngineSemanticModelNotAcceptDataProvider.TestCases),
            MemberType = typeof(EngineSemanticModelNotAcceptDataProvider))]
        public async Task CanBeRefactored_ButNotWithSameIdentifier_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false, true);
        }

        [Theory]
        [MemberData(nameof(EngineDuplicatedVariableDeclDataProvider.TestCases),
            MemberType = typeof(EngineDuplicatedVariableDeclDataProvider))]
        public async Task CanBeRefactored_ButNotWithSameVariableDecl_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false, true);
        }
    }
}
