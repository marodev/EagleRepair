using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationConditionalDataProvider
    {
        private const string Input = @"
using System.Globalization;

namespace Entry
{
    public class C
    {        
        public void M(bool b)
        {
            var s = string.Format(""platform={0}"", b ? ""true"" : ""false"");
        }
    }
}";

        private const string ExpectedOutput = @"
using System.Globalization;

namespace Entry
{
    public class C
    {        
        public void M(bool b)
        {
            var s = $""platform={(b ? ""true"" : ""false"")}"";
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
