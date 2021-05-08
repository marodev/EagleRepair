using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingMultipleAccessDataProvider
    {
        private const string InputAndExpectedOutput = @"
namespace Entry
{
    public class UsePatternMatchingMultipleAccessDataProvider
    {
        public int M(object o)
        {
            var s = o as string;
            var newValue = s == null ? ""null"" : s.Substring(0);
            
            if (s != null)
            {
                return s.Length;
            }
            
            return 0;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
