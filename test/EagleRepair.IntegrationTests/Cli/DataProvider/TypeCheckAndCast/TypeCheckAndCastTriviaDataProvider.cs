using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastTriviaDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {
        public bool M(object o)
        {
            if (o is string)
            {
                return ((string) o).Length > 2;
            }

            return false;
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {
        public bool M(object o)
        {
            if (o is string s)
            {
                return s.Length > 2;
            }

            return false;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
