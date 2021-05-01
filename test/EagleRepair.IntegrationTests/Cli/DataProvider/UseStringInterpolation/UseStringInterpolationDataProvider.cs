using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public class UseStringInterpolationDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(object o)
        {
            const string disneyLand = ""disneyLand"";
            Console.WriteLine(string.Format(""Hi {0}! Wanna go to {1}?"", o, disneyLand));
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(object o)
        {
            const string disneyLand = ""disneyLand"";
            Console.WriteLine(""Hi {o}! Wanna go to {disneyLand}?"");
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
