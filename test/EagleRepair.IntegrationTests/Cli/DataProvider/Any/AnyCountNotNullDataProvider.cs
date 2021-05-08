using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyCountNotNullDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;

namespace Entry
{
    public class C
    {
        public void M()
        {
            var names = new List<string>();
            
            if (names.Count != 0 && names.Count % 30 == 0)
            {
                // do something
            }
        }
    }
}";


        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public void M()
        {
            var names = new List<string>();
            
            if (names.Any() && names.Count % 30 == 0)
            {
                // do something
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
