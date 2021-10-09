using System.Threading.Tasks;
using EagleRepair.IntegrationTests.Cli.DataProvider.UseCollectionsCountProperty;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public class UseCollectionsCountPropertyTests
    {
        [Theory]
        [MemberData(nameof(UseCollectionsCountPropertyArrayDataProvider.TestCases),
            MemberType = typeof(UseCollectionsCountPropertyArrayDataProvider))]
        public async Task UseCountMethodInArray_ReturnsLengthProperty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }

        [Theory]
        [MemberData(nameof(UseCollectionsCountPropertyListDataProvider.TestCases),
            MemberType = typeof(UseCollectionsCountPropertyListDataProvider))]
        public async Task UseCountMethodInList_ReturnsCountProperty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
        
        [Theory]
        [MemberData(nameof(UseCollectionCoutPropertyAssignmentDataProvider.TestCases),
            MemberType = typeof(UseCollectionCoutPropertyAssignmentDataProvider))]
        public async Task UseCountMethodInAssignment_ReturnsCountProperty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
        
        [Theory]
        [MemberData(nameof(UseCollectionsCountPropertyEqualsDataProvider.TestCases),
            MemberType = typeof(UseCollectionsCountPropertyEqualsDataProvider))]
        public async Task UseCountMethodInEquals_ReturnsCountProperty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
        
        [Theory]
        [MemberData(nameof(UseCollectionsCountPropertyParameterDataProvider.TestCases),
            MemberType = typeof(UseCollectionsCountPropertyParameterDataProvider))]
        public async Task UseCountMethodAsParameter_ReturnsCountProperty(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }       
        
        [Theory]
        [MemberData(nameof(UseCollectionsCountPropertyIEnumerableDataProvider.TestCases),
            MemberType = typeof(UseCollectionsCountPropertyIEnumerableDataProvider))]
        public async Task UseCountMethodInIEnumerable_ReturnsInput(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree);
        }
        
        [Theory]
        [MemberData(nameof(UseCollectionsCountPropertyLinqDataProvider.TestCases),
            MemberType = typeof(UseCollectionsCountPropertyLinqDataProvider))]
        public async Task UseCountMethodWithLinq_ReturnsInput(string inputTree, string expectedTree)
        {
            await TestExecutor.Run(inputTree, expectedTree, false);
        }

    }
}
