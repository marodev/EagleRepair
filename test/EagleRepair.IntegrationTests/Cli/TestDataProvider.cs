using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli
{
    public static class TestDataProvider
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {UseMethodAnyInput, UseMethodAnyExpected};
        }

        private const string UseMethodAnyInput = @"
            using System;
            using System.Collections.Generic;

            public class FooClass
            {
                public void FooMethod()
                {
                    List<string> myList = new();

                    if (myList.Count > 0)
                    {
                        Console.WriteLine(""List is not empty"");
                    }
                }
            }";


        private const string UseMethodAnyExpected = @"
            using System;
            using System.Collections.Generic;

            public class FooClass
            {
                public void FooMethod()
                {
                    List<string> myList = new();

                    if (myList.Any())
                    {
                        Console.WriteLine(""List is not empty"");
                    }
                }
            }";
    }
}