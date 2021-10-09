using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqCountMethodDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public void M(IEnumerable<string> strings)
        {
            var list = new List<object>();
            var o = new object();
            var number = list.Count(l => l.Equals(o));

            var n = strings.Count();
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
