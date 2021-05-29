using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqWhereCountDataProvider
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

            var result = list.Where(i => i is not null && i.ToString().Equals(""foo"")).Count();

            if(list.Where(i => i is not null && i.ToString().Equals(""foo"")).Count() == 0) {
                // do something
            }

            return result > 0;
        }
    }
}";

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

            var result = list.Count(i => i is not null && i.ToString().Equals(""foo""));

            if(!list.Any(i => i is not null && i.ToString().Equals(""foo""))) {
                // do something
            }

            return result > 0;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
