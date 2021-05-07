using System;
using System.Net;
using Xunit;

namespace EagleRepair.IntegrationTests.Url
{
    public class TestExecutor
    {
        public static void Run(string url)
        {
            // Arrange
            var uri = new UriBuilder(url).Uri;
            // Act
            var exists = EndpointExists(uri);
            // Assert
            Assert.True(exists);
        }

        private static bool EndpointExists(Uri address)
        {
            try
            {
                using var client = new MyClient();
                client.DownloadStringAsync(address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private class MyClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var req = base.GetWebRequest(address);
                req.Method = "HEAD";
                return req;
            }
        }
    }
}
