using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseCollectionsCountProperty
{
    public class UseCollectionsCountPropertyEqualsDataProvider
    {


        private const string Input = @"
using System;
using System.Collections.Generic;

namespace Entry
{
    public class C
    {        
        public bool M()
        {
            var b = GetObjects().Count() == 1 || GetObjects().Count() == 2;
            return b;
        }

        public IList<object> GetObjects()
        {
            return new List<object>();
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
        public bool M()
        {
            var b = GetObjects().Count == 1 || GetObjects().Count == 2;
            return b;
        }

        public IList<object> GetObjects()
        {
            return new List<object>();
        }
    }
}";


        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
