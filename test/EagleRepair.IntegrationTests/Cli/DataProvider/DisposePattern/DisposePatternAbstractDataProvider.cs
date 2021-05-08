using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.DisposePattern
{
    public static class DisposePatternAbstractDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;

namespace Entry
{
    public abstract class DisposeAbstract : IDisposable
    {
        public abstract void Dispose();
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}


