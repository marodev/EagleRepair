using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public class SimplifyIEnumerableLinqWhereSingleOrDefaultDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereSingle
    {
        public bool FooMethod()
        {
            var list = new List<int>();

            var result = list.Where(i => i > 0).SingleOrDefault();

            return result > 42;
        }
    }
}";

        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereSingle
    {
        public bool FooMethod()
        {
            var list = new List<int>();

            var result = list.SingleOrDefault(i => i > 0);

            return result > 42;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
