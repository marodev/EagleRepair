using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public static class MergeSequentialChecksNotNullAndNullDataProvider
    {
        private const string InputAndExpectedOutput = @"
namespace Entry
{
    public class C
    {
        public class NestedClass
        {
            public string Foo()
            {
                return null;
            }
        }
        public static void M(NestedClass n)
        {
            if (n  != null && n.Foo() == null)
            {
                
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
