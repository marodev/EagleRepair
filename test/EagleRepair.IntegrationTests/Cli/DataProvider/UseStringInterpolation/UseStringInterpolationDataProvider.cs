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
        public void M(object o, int[] arr)
        {
            const string disneyLand = ""disneyLand"";
            Console.WriteLine(string.Format(""Hi {0}! Wanna go to {1}? In {2} seconds. {3}"", o, disneyLand, disneyLand.Length, arr.GetType()));
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(object o, int[] arr)
        {
            const string disneyLand = ""disneyLand"";
            Console.WriteLine($""Hi {o}! Wanna go to {disneyLand}? In {disneyLand.Length} seconds. {arr.GetType()}"");
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
