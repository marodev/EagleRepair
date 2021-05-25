using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqCountPropertyDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public void M()
        {
            var list = new List<object>();
            var number = list.Count();
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
        public void M()
        {
            var list = new List<object>();
            var number = list.Count;
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
