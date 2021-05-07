using System.Collections.Generic;
using EagleRepair.Ast.Url;

namespace EagleRepair.IntegrationTests.Url.DataProvider
{
    public class SonarQubeUrlDataProvider
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {SonarQube.BaseUrl};
        }
    }
}
