using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseCollectionsCountProperty
{
    public class UseCollectionsCountPropertyParameterDataProvider
    {


        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public int M(int x)
        {
            return 42;
        }

        public int M2()
        {
            return M(GetObjects().Count());
        }

        public IList<object> GetObjects()
        {
            return new List<object>();
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public int M(int x)
        {
            return 42;
        }

        public int M2()
        {
            return M(GetObjects().Count);
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
