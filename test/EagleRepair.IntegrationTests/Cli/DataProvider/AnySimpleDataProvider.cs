using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider
{
    public static class AnySimpleDataProvider
    {
        private const string UseMethodAnyWithoutUsingDirectiveInput = @"
using System;
using System.Collections.Generic;

public class FooClass
{
    public void FooMethod()
    {
        List<int> myList = new();

        if (myList.Count > 0)
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Count() > 0)
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Count() >= 1)
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Count() == 0)
        {
            Console.WriteLine(""List is empty"");
        }

        if (myList.Count() <= 0 && myList.Count() == 0)
        {
            Console.WriteLine(""List is empty"");
        }



        while (myList.Count < 1)
        {
            Console.WriteLine(""List is empty"");
        }
    }
}";


        private const string UseMethodAnyWithoutUsingDirectiveExpected = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class FooClass
{
    public void FooMethod()
    {
        List<int> myList = new();

        if (myList.Any())
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Any())
        {
            Console.WriteLine(""List is not empty"");
        }
        if (myList.Any())
        {
            Console.WriteLine(""List is not empty"");
        }
        if (!myList.Any())
        {
            Console.WriteLine(""List is empty"");
        }

        if (!myList.Any() && !myList.Any())
        {
            Console.WriteLine(""List is empty"");
        }



        while (!myList.Any())
        {
            Console.WriteLine(""List is empty"");
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                UseMethodAnyWithoutUsingDirectiveInput, UseMethodAnyWithoutUsingDirectiveExpected
            };
        }
    }
}
