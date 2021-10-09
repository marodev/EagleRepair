using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastReservedKeywordDataProvider
    {
        private const string Input = @"
using System;
using System.Globalization;

namespace Entry
{
    public class C
    {
        public class Public
        {
        }

        public void M(object o)
        {
            if (o is Public)
            {
                Console.WriteLine((Public)o);
            }
        }
    }
}";

        private const string ExpectedOutput = @"
using System;
using System.Globalization;

namespace Entry
{
    public class C
    {
        public class Public
        {
        }

        public void M(object o)
        {
            if (o is Public @public)
            {
                Console.WriteLine(@public);
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
