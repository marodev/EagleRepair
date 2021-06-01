using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingGenericsDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {
        public string M(object o)
        {
            var casted = o as Nestedclass<Guid>;
            if (casted != null)
            {
                return casted.ToString();
            }

            return string.Empty;
        }

        public class Nestedclass<T> { }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {
        public string M(object o)
        {
            if (o is Nestedclass<Guid> casted)
            {
                return casted.ToString();
            }

            return string.Empty;
        }

        public class Nestedclass<T> { }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
