using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationNumberFormatDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M()
        {
            var str1 = string.Format(""{0:0.##}"", 123.4567);
            var str2 = string.Format(""{0:N2}%"", 42.5555555);
            var str3 = string.Format(""{0,-35}"", ""hello"");
            var str4 = string.Format(""---> {0,-10} | {1,5} <---"", ""James Bond"", 007);
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M()
        {
            var str1 = $""{123.4567:0.##}"";
            var str2 = $""{42.5555555:N2}%"";
            var str3 = $""{""hello"", -35}"";
            var str4 = $""---> {""James Bond"", -10} | {007, 5} <---"";
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
