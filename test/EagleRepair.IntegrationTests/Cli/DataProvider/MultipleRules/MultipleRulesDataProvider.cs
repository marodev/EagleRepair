using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MultipleRules
{
    public static class MultipleRulesDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class MultipleFixes
    {
        public void M(object o)
        {
            var s = o as string;
            if (s != null)
            {
                // use pattern matching
            }

            var value = """";

            if (value != null && value.Length > 0)
            {
                // use string.IsNullOrEmpty
            }
        }
    }
}";

        private const string ExpectedOutput = @"
namespace Entry
{
    public class MultipleFixes
    {
        public void M(object o)
        {
            if (o is string s)
            {
                // use pattern matching
            }

            var value = """";

            if (!string.IsNullOrEmpty(value))
            {
                // use string.IsNullOrEmpty
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
