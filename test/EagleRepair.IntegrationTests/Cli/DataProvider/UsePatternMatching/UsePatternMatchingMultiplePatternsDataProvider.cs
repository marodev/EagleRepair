using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingMultiplePatternsDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C1
    {
        public class C2 {}
        public class C3 {}
        
        public void M(object o)
        {
            var str = o as string;
            var c2 = o as C2;
            var c3 = o as C3;

            if (str != null)
            {
                // str != null
            } else if (c2 != null)
            {
                // c2 != null
            } else if (c3 != null)
            {
                // c3 != null
            }
            else
            {
                // else
            }
        }
    }
}";

        // TODO: we might consider using a switch statement in a future release
        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C1
    {
        public class C2 {}
        public class C3 {}
        
        public void M(object o)
        {
            if (o is string str)
            {
                // str != null
            } else if (o is C2 c2)
            {
                // c2 != null
            } else if (o is C3 c3)
            {
                // c3 != null
            }
            else
            {
                // else
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
