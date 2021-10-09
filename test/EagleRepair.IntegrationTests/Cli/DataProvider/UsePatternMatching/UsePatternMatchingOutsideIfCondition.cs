using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingOutsideIfCondition
    {
        private const string InputAndExpectedOutput = @"
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Entry
{
    public class C
    {
        public void M(object o)
        {
           var foo = o as string;
           CallMe(foo != null);
        }
        
        public void CallMe(bool b) { }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
