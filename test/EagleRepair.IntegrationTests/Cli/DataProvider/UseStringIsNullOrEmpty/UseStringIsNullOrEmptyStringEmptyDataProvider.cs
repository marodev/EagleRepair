using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UseStringIsNullOrEmpty
{
    public static class UseStringIsNullOrEmptyStringEmptyDataProvider
    {
        private const string Input = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(string s)
        {
            Console.WriteLine(string.Empty);

            if(s.Equals(string.Empty)) {
                // do something
            }

            if(!s.Equals(string.Empty)) {
                // do something
            }

            if(s != null && s.Equals(string.Empty)) {
                // do something
            }

            if(s != null && !s.Equals(string.Empty)) {
                // do something
            }
        }
    }
}";

        private const string ExpectedOutput = @"
using System;

namespace Entry
{
    public class C
    {        
        public void M(string s)
        {
            Console.WriteLine(string.Empty);

            if(string.IsNullOrEmpty(s)) {
                // do something
            }

            if(!string.IsNullOrEmpty(s)) {
                // do something
            }

            if(string.IsNullOrEmpty(s)) {
                // do something
            }

            if(!string.IsNullOrEmpty(s)) {
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
