using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingNotInConditionDataProvider
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
            var list = o as List<string>;
            Debug.Assert(list != null);
            
            foreach (var s in list)
            {
                // do something
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
