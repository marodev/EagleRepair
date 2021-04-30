using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public class TypeCheckAndCastSimpleDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class TypeCheckAndCast
    {
        public class NestedClass
        {
            public string StringValue { get; set; }
        }
        
        public int FooMethod()
        {
            var o = new object();

            if (o is string)
            {
                Console.WriteLine(""It's a string!"");
                return ((string) o).Length;
            } else if (o is NestedClass)
            {
                Console.WriteLine(""It's a NestedClass!"");
                return ((NestedClass) o).StringValue.Length;
            } else if (o is int)
            {
                Console.WriteLine(""It's an int!"");
                return i;
            }

            return 0;
        }
    }
}
";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class TypeCheckAndCast
    {
        public class NestedClass
        {
            public string StringValue { get; set; }
        }
        
        public int FooMethod()
        {
            var o = new object();

            if (o is string s)
            {
                Console.WriteLine(""It's a string!"");
                return s.Length;
            } else if (o is NestedClass nestedClass)
            {
                Console.WriteLine(""It's a NestedClass!"");
                return nestedClass.StringValue.Length;
            } else if (o is int)
            {
                Console.WriteLine(""It's an int!"");
                return i;
            }

            return 0;
        }
    }
}
";       
        
        
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                Input, ExpectedOutput
            };
        }
    }
}
