using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyNullableDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Collections.Generic;

namespace Entry
{
    public class NullableDataProvider
    {
        public class Nested
        {
            public IList<int> PeriodicBackups;
        }
        
        public void M(Nested n)
        {
            if (n?.PeriodicBackups.Count > 0)
            {
                // do something
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
