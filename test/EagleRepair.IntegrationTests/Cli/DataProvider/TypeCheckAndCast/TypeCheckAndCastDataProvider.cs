using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public class ComplexDataProvider
    {
        private const string input = @"
using System;

namespace Entry
{
    public class TypeCheckAndCast
    {
        public class NestedClass
        {
            public string StringValue { get; set; }
        }
        
        public object FooMethod()
        {
            var o = new object();

            if (o is string)
            {
                Console.WriteLine(""It's a string!"");
                return ((string) o).Length;
            } else if (o is NestedClass)
            {
                Console.WriteLine(""It's an object!"");
                return ((NestedClass) o).StringValue;
            }

            return new object();
        }
    }
}
";

        private const string expectedOutput = @"
using System;

namespace Entry
{
    public class TypeCheckAndCast
    {
        public class NestedClass
        {
            public string StringValue { get; set; }
        }
        
        public object FooMethod()
        {
            var o = new object();

            if (o is string)
            {
                Console.WriteLine(""It's a string!"");
                return ((string) o).Length;
            } else if (o is NestedClass)
            {
                Console.WriteLine(""It's an object!"");
                return ((NestedClass) o).StringValue;
            }

            return new object();
        }
    }
}
";       
        
        
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                input, expectedOutput
            };
        }
    }
}
