using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.NullChecksShouldNotBeUsedWithIs
{
    public class NullChecksShouldNotBeUsedWithIsCustomTypeDataProvider
    {
        private const string Input = @"
namespace Entry
{
    public class C
    {
        public class Vehicle {}
        public class Car : Vehicle {}

        public void M(Vehicle v)
        {
            if (v != null && v is Car)
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
        public class Car : Vehicle {}

        public void M(Vehicle v)
        {
            if (v is Car)
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
