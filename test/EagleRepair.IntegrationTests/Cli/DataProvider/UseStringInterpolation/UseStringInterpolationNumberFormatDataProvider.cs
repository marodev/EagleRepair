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
            var str = string.Format(""{0:N2}%"", 42.5555555);
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
            var str = $""{42.5555555:N2}%"";
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
