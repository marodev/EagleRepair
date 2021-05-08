using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class UsePatternMatchingTests
    {
        [Theory]
        [MemberData(nameof(UsePatternMatchingDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingDataProvider))]
        public async Task UseSuboptimalCode_ReturnsPatternMatching(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingNegativeDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingNegativeDataProvider))]
        public async Task UseDifferentVariableInNullCheck_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(UsePatternMatchingComplexDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingComplexDataProvider))]
        public async Task UseMethodAccess_ExplicitlyConvertAs_ReturnsPatternMatching(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }        
        
        [Theory]
        [MemberData(nameof(UsePatternMatchingMultipleAccessDataProvider.TestCases),
            MemberType = typeof(UsePatternMatchingMultipleAccessDataProvider))]
        public async Task UseAsType_VariableAccessedMultipleTimesNotInCondition_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }
    }
}
