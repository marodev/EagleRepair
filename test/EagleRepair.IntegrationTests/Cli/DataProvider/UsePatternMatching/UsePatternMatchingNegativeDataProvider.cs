using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public class UsePatternMatchingNegativeDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(object o)
        {
            var s = o as string;
            if (o != null) {
                Console.WriteLine($""Hi. {s}"");
            }
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
