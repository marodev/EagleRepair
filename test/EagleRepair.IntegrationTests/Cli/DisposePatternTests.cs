using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class DisposePatternTests
    {
        [Theory]
        [MemberData(nameof(DisposePatternDataProvider.TestCases),
            MemberType = typeof(DisposePatternDataProvider))]
        public async Task FindDispose_AddProtectedDispose_ReturnsDisposePattern(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(DisposePatternSealedDataProvider.TestCases),
            MemberType = typeof(DisposePatternSealedDataProvider))]
        public async Task FindDispose_CanBeSealed_ReturnsSealedClass(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(DisposePatternAlreadySealedDataProvider.TestCases),
            MemberType = typeof(DisposePatternAlreadySealedDataProvider))]
        public async Task FindDispose_IsAlreadySealed_ReturnsInput(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(DisposePatternAbstractDataProvider.TestCases),
            MemberType = typeof(DisposePatternAbstractDataProvider))]
        public async Task FindDispose_IsAbstract_ReturnsInput(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(DisposePatternNegativeDataProvider.TestCases),
            MemberType = typeof(DisposePatternNegativeDataProvider))]
        public async Task FindDispose_ImplementedCorrectly_ReturnsInput(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }
    }
}
