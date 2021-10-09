using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationAndConcatenationDataProvider
    {
        private const string Input = @"
using System.Globalization;

namespace Entry
{
    public class C
    {        
            var s = string.Format(""0 -> {0},\n""
            + ""1 -> {1}\n"",
            + 0, 1);
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, Input };
        }
    }
}
