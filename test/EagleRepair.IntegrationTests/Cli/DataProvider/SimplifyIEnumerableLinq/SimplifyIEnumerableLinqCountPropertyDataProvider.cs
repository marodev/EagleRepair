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

            var list2 = new List<object>();
            for(int i = 0; i < list2.Count(); i++) {
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
        public void M()
        {
            var list = new List<object>();
            var number = list.Count;

            var list2 = new List<object>();
            for(int i = 0; i < list2.Count; i++) {
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
