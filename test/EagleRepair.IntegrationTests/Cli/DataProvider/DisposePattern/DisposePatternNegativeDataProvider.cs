using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern
{
    public static class DisposePatternNegativeDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;

namespace Entry
{
    public class DisposeMe : IDisposable
    {
        // To detect redundant calls
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // cleanup
                Console.WriteLine(""disposed."");
            }

            _disposed = true;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
