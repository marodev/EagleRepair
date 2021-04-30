using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public class SimplifyIEnumerableLinqWhereSelect
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereSelect
    {
        public class NestedClass {}
        
        public static List<T> Foo<T>() where T : NestedClass
        {
            var list = new List<object>();
            var result1 = list.Where(element => element is T).Select(e => e as T);
            var result2 = list.Where(element => element is T).Select(e => (T) e);

            return new List<T>(result1);
        }
    }
}";

        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SimplifyIEnumerableLinqWhereSelect
    {
        public class NestedClass {}
        
        public static List<T> Foo<T>() where T : NestedClass
        {
            var list = new List<object>();
            var result1 = list.OfType<T>();
            var result2 = list.OfType<T>();

            return new List<T>(result1);
        }
    }
}";
        
        
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
