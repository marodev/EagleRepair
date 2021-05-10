using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern
{
    public class DisposePatternPartialDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public partial class C : IDisposable
    {
        public void Dispose()
        {
            // cleanup
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public partial class C : IDisposable
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
            }

            _disposed = true;
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
