using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.Any
{
    public static class AnyMultipleBinaryExprDataProvider
    {
        private const string Input = @"
using System;
using System.Collections.Generic;

public class FooClass
{
    public void FooMethod(IEnumerable<int> myList)
    {
        if (myList.Count() <= 0 && myList.Count() == 0 && myList.Count() < 1)
        {
            if (myList.Count() <= 0 && myList.Count() == 0 && myList.Count() < 1)
            {
                Console.WriteLine(""List is empty"");
            }
            Console.WriteLine(""List is empty"");
        }
    }
}";


        private const string ExpectedOutput = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class FooClass
{
    public void FooMethod(IEnumerable<int> myList)
    {
        if (!myList.Any() && !myList.Any() && !myList.Any())
        {
            if (!myList.Any() && !myList.Any() && !myList.Any())
            {
                Console.WriteLine(""List is empty"");
            }
            Console.WriteLine(""List is empty"");
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
