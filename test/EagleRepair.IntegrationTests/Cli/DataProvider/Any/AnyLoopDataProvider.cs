using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyLoopDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System.Collections.Generic;
using System;

namespace Entry
{
    public class C
    {
        public void M(IEnumerable<int> numbers)
        {
            for (var i = 0; i < numbers.Count(); i++)
            {
                Console.Write(numbers[i]);
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
