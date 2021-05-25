using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.NullChecksShouldNotBeUsedWithIs
{
    public static class NullChecksShouldNotBeUsedWithIsMemberAccessDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class C
    {
        public class Vehicle {}

        public class Car
        {
            public Vehicle V { get; set; }
        }

        public void M(Car c)
        {
            if (c.V != null && c.V is Vehicle v)
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
        public class Vehicle {}

        public class Car
        {
            public Vehicle V { get; set; }
        }

        public void M(Car c)
        {
            if (c.V is Vehicle v)
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
