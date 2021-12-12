using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastNewLineDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {
        public string M(object o)
        {
            if (o.GetHashCode() == 2)
            {
                return string.Empty;
            } else if (o is string)
            {
                var str = (string) o;

                try
                {
                    return str;
                } catch(Exception) {}
            }

            return string.Empty;
        }
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
            if (o.GetHashCode() == 2)
            {
                return string.Empty;
            } else if (o is string str)
            {
                try
                {
                    return str;
                } catch(Exception) {}
            }

            return string.Empty;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
