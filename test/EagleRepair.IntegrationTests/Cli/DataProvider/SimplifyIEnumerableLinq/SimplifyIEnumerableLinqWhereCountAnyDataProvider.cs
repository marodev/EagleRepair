using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public class SimplifyIEnumerableLinqWhereCountAnyDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public class NestedClass
        {
            public IEnumerable<int> Products;
        }

        public void FooMethod(NestedClass nc)
        {
            if (nc.Products.Where(n => n == 2).Count() > 0)
            {
                // do something
            }
        }
    }
}
";

        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public class NestedClass
        {
            public IEnumerable<int> Products;
        }

        public void FooMethod(NestedClass nc)
        {
            if (nc.Products.Any(n => n == 2))
            {
                // do something
            }
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
