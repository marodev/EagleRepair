using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastArrayDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {
        public int M(object o)
        {
            if (o is byte[])
            {
                return ((byte[]) o).Length;
            }

            if (o is System.Collections.Generic.List<int>)
            {
                return ((System.Collections.Generic.List<int>) o).Count;
            }
            return 0;
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {
        public int M(object o)
        {
            if (o is byte[] bytes)
            {
                return bytes.Length;
            }

            if (o is System.Collections.Generic.List<int> ints)
            {
                return ints.Count;
            }
            return 0;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
