using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqWhereFirstOrDefaultDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereFirst
    {
        public bool FooMethod()
        {
            var list = new List<int>();

            var result = list.Where(i => i > 0).FirstOrDefault();

            return result > 42;
        }
    }
}";

        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereFirst
    {
        public bool FooMethod()
        {
            var list = new List<int>();

            var result = list.FirstOrDefault(i => i > 0);

            return result > 42;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
