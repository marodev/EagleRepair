using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public class MergeSequentialChecksNullOrSmallerDataProvider
    {
        // TODO: might consider new C# 9 syntax:   if (s is not {Length: count})
        private const string InputAndExpectedOutput = @"
namespace Entry
{
    public class C
    {
        public void M(string s)
        {
            const int count = 2;
            if (s == null || s.Length != count)
                return;
            
            // Do something
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
