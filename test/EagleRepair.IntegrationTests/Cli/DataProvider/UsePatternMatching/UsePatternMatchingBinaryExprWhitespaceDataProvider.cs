using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingBinaryExprWhitespaceDataProvider
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
            return s != null && 42 == s.GetHashCode();
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
            return o is string s && 42 == s.GetHashCode();
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
