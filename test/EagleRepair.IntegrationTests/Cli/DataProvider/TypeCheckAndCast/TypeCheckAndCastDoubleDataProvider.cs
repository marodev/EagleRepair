using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.TypeCheckAndCast
{
    public static class TypeCheckAndCastDoubleDataProvider
    {
        private const string Input = @"
using System;
using System.Globalization;

namespace Entry
{
    public class C
    {
       public bool M()
       {
            if (primitiveValue is Double)
            {
                decimal doubleToDecimalR;
                if (decimal.TryParse(((Double)primitiveValue).ToString(""R"", CultureInfo.InvariantCulture), 
                    out doubleToDecimalR))
                {
                    return doubleToDecimalR;
                }
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
       public bool M()
       {
            if (primitiveValue is Double d)
            {
                decimal doubleToDecimalR;
                if (decimal.TryParse(d.ToString(""R"", CultureInfo.InvariantCulture), 
                    out doubleToDecimalR))
                {
                    return doubleToDecimalR;
                }
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
