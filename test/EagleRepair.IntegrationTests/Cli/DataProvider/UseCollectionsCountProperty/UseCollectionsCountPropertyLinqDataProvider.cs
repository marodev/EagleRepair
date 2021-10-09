using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseCollectionsCountProperty
{
    public class UseCollectionsCountPropertyLinqDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entry
{
    public class C
    {        
        public int M1(ICollection<int> collectionItems)
        {
            return collectionItems.Count(i => i > 2);
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
