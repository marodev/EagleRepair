using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class SimplifyLinqTests
    {
        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereAnyDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereAnyDataProvider))]
        public async Task SimplifyLinq_WhereAny_ReturnsAny(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereCountDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereCountDataProvider))]
        public async Task SimplifyLinq_WhereCount_ReturnsCount(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereFirstDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereFirstDataProvider))]
        public async Task SimplifyLinq_WhereFirst_ReturnsFirst(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereFirstOrDefaultDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereFirstOrDefaultDataProvider))]
        public async Task SimplifyLinq_WhereFirstOrDefault_ReturnsFirstOrDefault(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereSingleDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereSingleDataProvider))]
        public async Task SimplifyLinq_WhereSingle_ReturnsSingle(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereSingleOrDefaultDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereSingleOrDefaultDataProvider))]
        public async Task SimplifyLinq_WhereSingleOrDefault_ReturnsSingleOrDefault(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereSelectDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereSelectDataProvider))]
        public async Task SimplifyLinq_WhereSelect_ReturnsOfType(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereLastDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereLastDataProvider))]
        public async Task SimplifyLinq_WhereLast_ReturnsLast(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereNotPossible.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereNotPossible))]
        public async Task SimplifyLinq_WhereFuncThreeAny_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqCountPropertyDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqCountPropertyDataProvider))]
        public async Task SimplifyLinq_UsesCountMethod_ReturnsCountMember(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqCountMethodDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqCountMethodDataProvider))]
        public async Task SimplifyLinq_UsesCountMethod_ReturnsOriginal(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereWhereFirstOrDefaultDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereWhereFirstOrDefaultDataProvider))]
        public async Task SimplifyLinq_UsesTwoTimesWhereFirstOrDefault_ReturnsWhereFirstOrDefault(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereCountAnyDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereCountAnyDataProvider))]
        public async Task SimplifyLinq_UsesWhereAndCountGreaterZero_ReturnsAny(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(SimplifyIEnumerableLinqWhereFirstOrDefaultNotEmptyDataProvider.TestCases),
            MemberType = typeof(SimplifyIEnumerableLinqWhereFirstOrDefaultNotEmptyDataProvider))]
        public async Task SimplifyLinq_UsesWhereAndCFirstOrDefaultWithPredicate_ReturnsOriginal(string inputTree,
            string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }
    }
}
