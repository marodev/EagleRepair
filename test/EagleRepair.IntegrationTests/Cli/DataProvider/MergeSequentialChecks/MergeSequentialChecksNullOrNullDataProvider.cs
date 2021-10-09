using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public static class MergeSequentialChecksNullOrNullDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class C
    {
        public interface IA {}
        public class B : IA
        {
            public IA Parent;
        }
        public void M(B b)
        {
            if (b == null || b.Parent is null)
            {
                // do something
            }
        }
    }
}";

        private const string ExpectedOutput = @"
namespace Entry
{
    public class C
    {
        public interface IA {}
        public class B : IA
        {
            public IA Parent;
        }
        public void M(B b)
        {
            if (b?.Parent is null)
            {
                // do something
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
