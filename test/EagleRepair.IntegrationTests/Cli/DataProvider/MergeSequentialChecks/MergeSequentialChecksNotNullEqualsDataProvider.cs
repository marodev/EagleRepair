using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public static class MergeSequentialChecksNotNullEqualsDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class C
    {
        public void M(string s)
        {
            const int count = 2;
            if (s != null && s.Length == count)
                return;
            
            // Do something
        }
    }
}";

        private const string ExpectedOutput = @"
namespace Entry
{
    public class C
    {
        public void M(string s)
        {
            const int count = 2;
            if (s?.Length == count)
                return;
            
            // Do something
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
