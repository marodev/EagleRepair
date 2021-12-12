using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public class SimplifyIEnumerableLinqOfTypeNegativeDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public static void Foo(ICollection<object> o)
        {
            var strings = o.Where(so => so is string && so.GetHashCode() == 2).Select(so => so as string);
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
