using EagleRepair.IntegrationTests.Url.DataProvider;
using Xunit;

namespace EagleRepair.IntegrationTests.Url
{
    public class UrlExistsTests
    {
        [Theory]
        [MemberData(nameof(ReSharperUrlDataProvider.TestCases), MemberType = typeof(ReSharperUrlDataProvider))]
        public void ReSharperBaseUrl_Exists(string url)
        {
            TestExecutor.Run(url);
        }
        
        [Theory]
        [MemberData(nameof(SonarQubeUrlDataProvider.TestCases), MemberType = typeof(SonarQubeUrlDataProvider))]
        public void SonarQubeBaseUrl_Exists(string url)
        {
            TestExecutor.Run(url);
        }
    }
}
