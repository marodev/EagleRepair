using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.NullPropagation
{
    public static class SimpleNullPropagationDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class NullPropagation
    {
        public class NestedClass
        {
            public void Fly(int i) {}
        }
        
        public void FooMethod(NestedClass c)
        {
            Console.WriteLine(""hi"");
            if (c != null)
            {
                c.Fly(42);
            }

            if (c != null)
            {
                c.Fly(42);
            }
        }
    }
}";

        private const string ExpectedOutput = @"
namespace Entry
{
    public class NullPropagation
    {
        public class NestedClass
        {
            public void Fly(int i) {}
        }
        
        public void FooMethod(NestedClass c)
        {
            Console.WriteLine(""hi"");
            c?.Fly(42);

            c?.Fly(42);
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
