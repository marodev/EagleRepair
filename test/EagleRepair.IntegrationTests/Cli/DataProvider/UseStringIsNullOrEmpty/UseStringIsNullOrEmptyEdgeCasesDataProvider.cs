using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringIsNullOrEmpty
{
    public class UseStringIsNullOrEmptyEdgeCasesDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(string s, object o)
        {
            if(s != null && s != """" && o != null) {
                // do something
            }
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(string s, object o)
        {
            if(!string.IsNullOrEmpty(s) && o != null) {
                // do something
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
