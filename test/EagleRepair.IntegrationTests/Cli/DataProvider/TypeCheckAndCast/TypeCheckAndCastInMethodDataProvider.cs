using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastInMethodDataProvider
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
                var str = (string) o;
                var length = str.Length.GetHashCode();
                return length > 2;
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
            if (o is string str)
            {
                var length = str.Length.GetHashCode();
                return length > 2;
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
