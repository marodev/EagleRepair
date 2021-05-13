using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingMethodInvocationDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class C
    {
        public class NestedClass { }

        public string M(object o)
        {
            var nested = o.ToString() as string;
            
            if (nested != null)
            {
                return nested;
            }

            return string.Empty;
        }
    }
}";

        private const string ExpectedOutput = @"
namespace Entry
{
    public class C
    {
        public class NestedClass { }

        public string M(object o)
        {
            if (o.ToString() is string nested)
            {
                return nested;
            }

            return string.Empty;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
