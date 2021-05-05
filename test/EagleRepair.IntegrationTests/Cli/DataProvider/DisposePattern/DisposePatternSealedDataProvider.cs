using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern
{
    public static class DisposePatternSealedDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C : IDisposable
    {
        public void Dispose()
        {
            // Cleanup
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public sealed class C : IDisposable
    {
        public void Dispose()
        {
            // Cleanup
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
