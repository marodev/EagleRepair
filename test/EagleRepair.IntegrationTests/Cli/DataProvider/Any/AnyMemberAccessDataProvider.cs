using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyMemberAccessDataProvider
    {
        private const string InputAndExpectedOutput = @"
using System;
using System.Collections.Generic;

public class FooClass
{
    public void FooMethod(IList<int> myList)
    {
        if (myList.Count > 0)
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Count > 0)
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Count >= 1)
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Count == 0)
        {
            Console.WriteLine(""List is empty"");
        }
        while (myList.Count < 1)
        {
            Console.WriteLine(""List is empty"");
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {InputAndExpectedOutput, InputAndExpectedOutput};
        }
    }
}
