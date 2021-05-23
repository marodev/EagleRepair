using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationControlSpacingDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Globalization;

namespace Entry
{
    public class C
    {        
        public void M()
        {
            string.Format(""{0, -"" + (n + 2) +  ""}{1}"", ""Year"", ""Population"");
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
