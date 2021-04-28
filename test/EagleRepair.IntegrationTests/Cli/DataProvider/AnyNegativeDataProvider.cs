using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider
{
    public static class AnyNegativeDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class SomeClass
    {
        private readonly NestedClass _nested = new();

        public class NestedClass
        {
            public NestedNestedClass Values()
            {
                return new NestedNestedClass();
            }

            public class NestedNestedClass
            {
                public int Count { get; set; }
            }
        }

        public void Foo()
        {
            var nestedNestedClass = new NestedClass.NestedNestedClass();

            if (nestedNestedClass.Count > 0)
            {
                Console.WriteLine(""Don't refactor me"");
            }
            
            if (_nested.Values().Count < 1)
            {
                Console.WriteLine(""Don't refactor me"");
            }
        }
    }     
}
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, Input};
        }
    }
}
