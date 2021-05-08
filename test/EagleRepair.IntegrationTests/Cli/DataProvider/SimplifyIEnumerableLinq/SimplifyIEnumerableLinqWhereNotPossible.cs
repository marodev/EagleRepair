using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public static class SimplifyIEnumerableLinqWhereNotPossible
    {
        private const string InputAndExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyLinqFun
    {
        public void M()
        {
            var numbers = new List<int>();
            var result = numbers.Where((t, i) => true).Any();
        }
    }
}";
        

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}


