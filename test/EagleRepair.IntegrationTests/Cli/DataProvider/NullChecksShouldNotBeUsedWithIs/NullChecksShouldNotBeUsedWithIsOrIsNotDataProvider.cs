using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.NullChecksShouldNotBeUsedWithIs
{
    public static class NullChecksShouldNotBeUsedWithIsOrIsNotDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class C
    {
        public void M(object s)
        {
            if (s == null || !(s is string))
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
        public void M(object s)
        {
            if (!(s is string))
            {
                // do something
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
