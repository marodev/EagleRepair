using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastToNullDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;

namespace Entry
{
    public class TypeCheckAndCast
    {        
        public void FooMethod()
        {
            var o = new object();

            if (o is null)
            {
                return;
            }
        }
    }
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
