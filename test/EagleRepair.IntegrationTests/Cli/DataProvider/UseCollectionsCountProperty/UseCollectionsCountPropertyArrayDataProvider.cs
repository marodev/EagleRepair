using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseCollectionsCountProperty
{
    public class UseCollectionsCountPropertyArrayDataProvider
    {
        
        private const string Input = @"
using System;
using System.Collections.Generic;

namespace Entry
{
    public class C
    {        
        public int M(string[] foos)
        {
            return foos.Count();
        }
    }
}";

        private const string ExpectedOutput = @"
using System;
using System.Collections.Generic;

namespace Entry
{
    public class C
    {        
        public int M(string[] foos)
        {
            return foos.Length;
        }
    }
}";


        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}


