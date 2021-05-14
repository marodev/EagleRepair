using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingTriviaDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public bool M(object o)
        {
            var s = o as string;

            if (s != null && 42 == s.GetHashCode()) {
                return true;
            }

            return false;
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public bool M(object o)
        {
            if (o is string s && 42 == s.GetHashCode()) {
                return true;
            }

            return false;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
