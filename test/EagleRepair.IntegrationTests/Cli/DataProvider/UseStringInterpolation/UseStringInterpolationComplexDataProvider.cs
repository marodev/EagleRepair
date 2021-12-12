using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationComplexDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Globalization;

namespace Entry
{
    public class C
    {        
        public void M()
        {
            var s = string.Format(CultureInfo.CurrentCulture, ""<DebugInfo({0}: {1}, {2}, {3}, {4})>"", 0, 1, 2, 3, 4);
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
