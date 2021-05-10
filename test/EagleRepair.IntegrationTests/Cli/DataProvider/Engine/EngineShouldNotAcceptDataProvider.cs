using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Engine
{
    public static class EngineShouldNotAcceptDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Diagnostics;

namespace Entry
{
    public class C
    {
        public void M(object o)
        {
            var s = o as string;
            
            Debug.Assert(s != null);
            Foo(s);
        }

        public void Foo(string s)
        {
            // do something
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
