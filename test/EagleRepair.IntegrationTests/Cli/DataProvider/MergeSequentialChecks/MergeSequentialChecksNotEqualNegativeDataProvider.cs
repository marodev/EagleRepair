using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public static class MergeSequentialChecksNotEqualNegativeDataProvider
    {
        private const string InputAndExpectedOutput = @"
namespace Entry
{
    public class C
    {
        public void M(string input)
        {
            var s = ""d"";
            if (s != """" && s.Length == s.Length)
                return;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
