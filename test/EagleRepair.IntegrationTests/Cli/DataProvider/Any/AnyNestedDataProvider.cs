using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyNestedDataProvider
    {
        private const string Input = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public class Program2
    {
        private readonly NestedClass _nested = new();

        private class NestedClass
        {
            public IEnumerable<int> Values()
            {
                yield return 42;
            }
        }

        public void Foo()
        {
            if (_nested.Values().Count() < 1)
            {
                Console.WriteLine(""List is empty"");
            } else {
                Console.WriteLine(""List is not empty"");
            }
        }
   }     
}
";

        private const string ExpectedOutput = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public class Program2
    {
        private readonly NestedClass _nested = new();

        private class NestedClass
        {
            public IEnumerable<int> Values()
            {
                yield return 42;
            }
        }

        public void Foo()
        {
            if (!_nested.Values().Any())
            {
                Console.WriteLine(""List is empty"");
            } else {
                Console.WriteLine(""List is not empty"");
            }
        }
   }     
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
