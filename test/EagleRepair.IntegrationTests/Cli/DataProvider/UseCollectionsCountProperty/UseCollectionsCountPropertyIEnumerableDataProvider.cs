using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseCollectionsCountProperty
{
    public class UseCollectionsCountPropertyIEnumerableDataProvider
    {
        
        private const string Input = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {        
        public int M1(ICollection<string> collectionItems)
        {
            return collectionItems.Count() - 1;
        }
        public int M2(IEnumerable<string> iEnumerableItems)
        {
            return iEnumerableItems.Count();
        }
    }
}";
        private const string ExpectedOutput = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {        
        public int M1(ICollection<string> collectionItems)
        {
            return collectionItems.Count - 1;
        }
        public int M2(IEnumerable<string> iEnumerableItems)
        {
            return iEnumerableItems.Count();
        }
    }
}";
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}

