using System.Collections.Generic;
using EagleRepair.Ast.Url;

namespace EagleRepair.IntegrationTests.Url.DataProvider
{
    public class ReSharperUrlDataProvider
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { ReSharper.BaseUrl };
        }
    }
}
