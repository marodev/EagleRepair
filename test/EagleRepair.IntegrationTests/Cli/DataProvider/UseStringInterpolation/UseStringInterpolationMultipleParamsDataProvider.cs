using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationMultipleParamsDataProvider
    {
        private const string InputAndExpectOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M()
        {
            var value = ""value foo"";
            var addition = ""addition foo"";

            // value foo addition foo
            var result = string.Format(value, addition);
        }
    }
}";


        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectOutput, InputAndExpectOutput};
        }
    }
}
