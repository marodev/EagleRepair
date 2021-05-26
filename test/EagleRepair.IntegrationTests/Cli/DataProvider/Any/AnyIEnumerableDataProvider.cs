using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyIEnumerableDataProvider
    {
        private const string Input = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SomeOtherClass
    {
        public bool Foo(IEnumerable<int> numbers)
        {
            var enumerable = numbers as int[] ?? numbers.ToArray();
            var isTrue = enumerable.Count(n => n > 42) > 0;
            return numbers.Count() > 0;
        }
    }
}";


        private const string ExpectedOutput = @"
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class SomeOtherClass
    {
        public bool Foo(IEnumerable<int> numbers)
        {
            var enumerable = numbers as int[] ?? numbers.ToArray();
            var isTrue = enumerable.Any(n => n > 42);
            return numbers.Any();
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
