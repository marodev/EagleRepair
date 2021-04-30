using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqWhereAnyDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereAny
    {
        public bool FooMethod()
        {
            var list = new List<object>();

            var result = list.Where(i => i is not null && i.ToString().Equals(""foo"")).Any();

            return result;
        }
    }
}
";

        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereAny
    {
        public bool FooMethod()
        {
            var list = new List<object>();

            var result = list.Any(i => i is not null && i.ToString().Equals(""foo""));

            return result;
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, Input};
        }
    }
}
