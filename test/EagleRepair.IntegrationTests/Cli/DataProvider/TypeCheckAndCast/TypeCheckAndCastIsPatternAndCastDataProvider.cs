using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastIsPatternAndCastDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {
        public void Foo(int i)
        {
        }
        
        public object M(object o)
        {
            string solution = string.Empty;
            if (o is int)
            {
                Foo((int) o);
            }
            
            if (o is string)
            {
                solution = (string) o;
            }

            return solution;
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {
        public void Foo(int i)
        {
        }
        
        public object M(object o)
        {
            string solution = string.Empty;
            if (o is int i)
            {
                Foo(i);
            }
            
            if (o is string s)
            {
                solution = s;
            }

            return solution;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
