using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Engine
{
    // for now not supported. May change in the future
    public static class EngineDuplicatedVariableDeclDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Diagnostics;

namespace Entry
{
    public class C
    {
        public void Transform(bool b) {}
        public void Transform(byte b) {}

        public void M(object o)
        {
            if (o is bool)
            {
                Transform((bool) o);
            }

            if (o is byte)
            {
                Transform((byte) o);
            }
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { InputAndExpectedOutput, InputAndExpectedOutput };
        }
    }
}
