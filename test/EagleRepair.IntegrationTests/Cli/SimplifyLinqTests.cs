using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class SimplifyLinqTests
    {
        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereAnyDataProvider.TestCases), MemberType = typeof(SimplifyIEnumerableLinqWhereAnyDataProvider))]
        public async Task SimplifyLinq_WhereAny_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }        
        
        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereCountDataProvider.TestCases), MemberType = typeof(SimplifyIEnumerableLinqWhereCountDataProvider))]
        public async Task SimplifyLinq_WhereCount_ReturnsCount(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
    }
}
