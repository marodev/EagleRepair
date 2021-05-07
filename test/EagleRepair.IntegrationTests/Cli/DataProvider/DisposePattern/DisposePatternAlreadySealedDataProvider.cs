using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern
{
    public class DisposePatternAlreadySealedDataProvider
    {
        private const string InputAndExpectedOutput = @"
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
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
