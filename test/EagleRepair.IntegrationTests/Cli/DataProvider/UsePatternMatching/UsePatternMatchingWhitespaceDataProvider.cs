using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingWhitespaceDataProvider
    {
        private const string Input = @"
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var arr = obj as CustomArray;

            if (arr != null)
                return Equals(_in, arr._inner);

            return Equals(_in, obj);
        }
";

        private const string ExpectedOutput = @"
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is CustomArray arr)
                return Equals(_in, arr._inner);

            return Equals(_in, obj);
        }
";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {Input, ExpectedOutput};
        }
    }
}
