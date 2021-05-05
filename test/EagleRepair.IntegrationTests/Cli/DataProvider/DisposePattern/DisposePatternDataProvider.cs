using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern
{
    public static class DisposePatternDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class Foo : DisposeMe
    {
        public void M()
        {
            this.Dispose();
        }
    }

    public class Foo2 : DisposeMe
    {
        public void M()
        {
            this.Dispose();
        }
    }
    
    public class DisposeMe : IDisposable, ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // cleanup
            Console.WriteLine(""disposed."");
            GC.SuppressFinalize(this);
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class Foo : DisposeMe
    {
        public void M()
        {
            this.Dispose();
        }
    }

    public class Foo2 : DisposeMe
    {
        public void M()
        {
            this.Dispose();
        }
    }

    public class DisposeMe : IDisposable, ICloneable
    {
        // To detect redundant calls
        private bool _disposed = false;

        public object Clone()
        {
            throw new NotImplementedException();
        }

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
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
