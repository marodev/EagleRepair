using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class TypeCheckAndCastTests
    {
        [Theory]
        [MemberData(nameof(TypeCheckAndCastSimpleDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastSimpleDataProvider))]
        public async Task UseTypeCheck_IfAndWhileStatement_ReturnsPatternExpr(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastToNullDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastToNullDataProvider))]
        public async Task UseTypeCheck_ChecksToNull_ReturnsNotRefactored(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(TypeCheckAndCastTriviaDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastTriviaDataProvider))]
        public async Task UseTypeCheck_InBinaryExpr_ReturnsPatternExprWithTrivia(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }        
        
        [Theory]
        [MemberData(nameof(TypeCheckAndCastInMethodDataProvider.TestCases),
            MemberType = typeof(TypeCheckAndCastInMethodDataProvider))]
        public async Task UseTypeCheck_InMethodCast_ReturnsPatternExprWithoutCast(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
