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


        [Theory]
        [MemberData(nameof(UseStringInterpolationComplexDataProvider.TestCases),
            MemberType = typeof(UseStringInterpolationComplexDataProvider))]
        public async Task UseStringFormat_UsesCultureInfo_ReturnsStringFormat(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(UseStringInterpolationMultipleParamsDataProvider.TestCases),
            MemberType = typeof(UseStringInterpolationMultipleParamsDataProvider))]
        public async Task UseStringFormat_UsesMultipleParameters_ReturnsStringFormat(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(UseStringInterpolationNumberFormatDataProvider.TestCases),
            MemberType = typeof(UseStringInterpolationNumberFormatDataProvider))]
        public async Task UseStringFormat_UsesFormatOptions_ReturnsInterpolatedString(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UseStringInterpolationKeepTriviaDataProvider.TestCases),
            MemberType = typeof(UseStringInterpolationKeepTriviaDataProvider))]
        public async Task UseStringFormat_UsesTrivia_ReturnsInterpolatedStringWithTrivia(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
