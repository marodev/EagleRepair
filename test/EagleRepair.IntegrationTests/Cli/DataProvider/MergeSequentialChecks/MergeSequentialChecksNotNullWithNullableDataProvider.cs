using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public static class MergeSequentialChecksNotNullWithNullableDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;

namespace Entry
{
    public class UnsafeExample
    {
        public DateTime? GetDate()
        {
            return null;
        }

        public void M()
        {
            var date = GetDate();

            if (date != null && date.Value >= DateTime.Now)
            {
                // do something
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
