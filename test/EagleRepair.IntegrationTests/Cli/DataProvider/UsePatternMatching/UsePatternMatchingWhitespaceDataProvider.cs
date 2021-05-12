using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingWhitespaceDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(object o)
        {
            if(true) {
                // ignore
            }

            var s = o as string;
            if (s != null) {
                Console.WriteLine($""Hi. {s}"");
            }
        }
    }
}
";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(object o)
        {
            if(true) {
                // ignore
            }

            if (o is string s) {
                Console.WriteLine($""Hi. {s}"");
            }
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
