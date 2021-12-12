using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.SimplifyIEnumerableLinq
{
    public class SimplifyIEnumerableLinqOfTypeDataProvider
    {
        private const string Input1 = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public static void Foo(ICollection<object> o)
        {
            var strings = o.Where(so => so is string).Select(so => so as string);
        }
    }
}";

        private const string Input2 = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public static IEnumerable<T> Foo<T>(ICollection<object> o) where T : C => o.Where(e => e is T).Select(e => e as T);
    }
}";

        private const string ExpectedOutput1 = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public static void Foo(ICollection<object> o)
        {
            var strings = o.OfType<string>();
        }
    }
}";

        private const string ExpectedOutput2 = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {
        public static IEnumerable<T> Foo<T>(ICollection<object> o) where T : C => o.OfType<T>();
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input1, ExpectedOutput1 };
            yield return new object[] { Input2, ExpectedOutput2 };
        }
    }
}
