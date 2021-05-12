using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.NullPropagation
{
    public static class NullPropagationNotValidDataProvider
    {
        private const string InputAndExpectedOutput = @"
namespace Entry
{
    public class SimplifyLinqFun
    {
        public class NestedClass { }
        
        public string M(object o)
        {
            if (o is NestedClass)
            {
                o.ToString();
            }

            return string.Empty;
        }
    }
}";


        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
