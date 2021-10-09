using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.NullPropagation
{
    public class NegativeNullPropagationDataProvider
    {
        private const string InputAndExpectedOutput = @"
namespace Entry
{
    public class NullPropagation
    {
        public class NestedClass
        {
            public void Fly(int i) {}
        }
        
        public void FooMethod(NestedClass c, string s)
        {
            if (c != null)
            {
                var length = s.Length;
            }
            Console.WriteLine(""hi"");
            if (c != null)
            {
                Console.WriteLine(""Don't refactor me."");
                c.Fly(42);
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
