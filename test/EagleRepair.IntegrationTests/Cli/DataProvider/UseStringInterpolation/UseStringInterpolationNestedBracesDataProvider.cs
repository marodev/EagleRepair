using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringInterpolation
{
    public static class UseStringInterpolationNestedBracesDataProvider
    {
        private const string InputAndExpectOutput = @"
namespace Entry
{
    public class C
    {        
        public void M(object json)
        {
            var s = String.Format(""{{ \""data\"" : {0} }}"", json);
        }
    }
}";


        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectOutput, InputAndExpectOutput };
        }
    }
}
