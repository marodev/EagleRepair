using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqWhereFirstOrDefaultNotEmptyDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public bool M()
        {
            var list = new List<int>();
            var result = list.Where(i => i > 0).FirstOrDefault(i => i != 10);

            return result > 42;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, Input };
        }
    }
}
