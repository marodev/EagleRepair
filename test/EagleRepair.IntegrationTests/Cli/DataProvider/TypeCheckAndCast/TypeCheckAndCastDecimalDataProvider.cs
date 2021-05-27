using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastDecimalDataProvider
    {
        private const string Input = @"
using System;
using System.Globalization;

namespace Entry
{
    public class C
    {
       public void Transform(decimal d){}

       public decimal M(object primitiveValue)
       {
            if (primitiveValue is Decimal) {
                Transform((Decimal) primitiveValue);
            }

            return new decimal(3.4);
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
       public void Transform(decimal d){}

       public decimal M(object primitiveValue)
       {
            if (primitiveValue is Decimal d) {
                Transform(d);
            }

            return new decimal(3.4);
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
