using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Engine
{
    public class EngineSemanticModelNotAcceptDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Diagnostics;

namespace Entry
{
    public class C
    {
        public void M(object o)
        {
            string f = null;
            if (o is string)
            {
                f = (string) o;
            }
            
            if (o is string)
            {
                f = (string) o;
            }

            return o;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
