using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationKeepTriviaDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M()
        {
            var strings = new List<string>
            {
                string.Format(""Point({0}, {1})"", 42, 8),
                string.Format(""Point({0}, {1})"", 21, 4)
            };
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
            var strings = new List<string>
            {
                $""Point({42}, {8})"",
                $""Point({21}, {4})""
            };
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
