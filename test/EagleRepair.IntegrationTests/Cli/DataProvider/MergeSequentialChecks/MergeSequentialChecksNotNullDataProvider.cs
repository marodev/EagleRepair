using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.MergeSequentialChecks
{
    public static class MergeSequentialChecksNotNullDataProvider
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
        public void M(B b, object o)
        {
            if (b != null && b.Parent != null && o is string)
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
        public void M(B b, object o)
        {
            if (b?.Parent != null && o is string)
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
