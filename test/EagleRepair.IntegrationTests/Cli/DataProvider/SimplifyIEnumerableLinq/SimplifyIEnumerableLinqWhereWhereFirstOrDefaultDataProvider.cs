using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqWhereWhereFirstOrDefaultDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereFirst
    {
        public bool M(IEnumerable<int> iEnumerable)
        {
            var result1 = iEnumerable
                .Where(n => n > 5)
                .Where(n => n < 10)
                .FirstOrDefault();
            
            var result2 = iEnumerable
                .Where(n => n > 5)
                .Where(n => n < 10)
                .FirstOrDefault(n => n > 6);
        }
    }
}";

        // TODO: not supported yet
        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereFirst
    {
        public bool M(IEnumerable<int> iEnumerable)
        {
            var result1 = iEnumerable
                .Where(n => n > 5)
                .FirstOrDefault(n => n < 10);
            
            var result2 = iEnumerable
                .Where(n => n > 5)
                .Where(n => n < 10)
                .FirstOrDefault(n => n > 6);
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, Input};
        }
    }
}
