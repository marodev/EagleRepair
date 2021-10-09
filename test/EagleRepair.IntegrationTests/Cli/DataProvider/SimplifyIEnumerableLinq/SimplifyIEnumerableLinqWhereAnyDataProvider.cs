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
        public class NestedClass
        {
            public IEnumerable<int> Products()
            {
                return null;
            }
        }

        public bool FooMethod(NestedClass nc)
        {
            var list = new List<object>();

            var result = list.Where(i => i is not null && i.ToString().Equals(""foo"")).Any();

            if (nc.Products().Where(n => n == 5).Count() == 0)
            {
                // do something
            }

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
        public class NestedClass
        {
            public IEnumerable<int> Products()
            {
                return null;
            }
        }

        public bool FooMethod(NestedClass nc)
        {
            var list = new List<object>();

            var result = list.Any(i => i is not null && i.ToString().Equals(""foo""));

            if (!nc.Products().Any(n => n == 5))
            {
                // do something
            }

            return result;
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
