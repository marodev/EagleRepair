using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringIsNullOrEmpty
{
    public static class UseStringIsNullOrEmptyNegativeDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(string s)
        {
            if(s != null && s.Length > 10) {
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
