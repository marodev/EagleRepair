using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EagleRepair.Ast;
using EagleRepair.Ast.Parser;
using Xunit;

namespace EagleRepair.IntegrationTests.Engine
{
    public class UnitTest1
    {
        [Theory]
        [MemberData(nameof(TestDataProvider.TestCases), MemberType = typeof(TestDataProvider))]
        public async Task UseMethodAnyTest(string inputTree, string expectedTree)
        {
            // Arrange
            var builder = DiContainerTestConfig.Builder();
            var mockSolutionParser = new MockSolutionParser(Utils.Workspace(inputTree));
            builder.Register(c => mockSolutionParser).As<ISolutionParser>();
            var container = builder.Build();
            await using var scope = container.BeginLifetimeScope();
            var engine = scope.Resolve<IEngine>();

            // Act
            var succeeded = await engine.RunAsync("ignore", null);
            var resultNode = await mockSolutionParser.Workspace().CurrentSolution.Projects
                .SelectMany(p => p.Documents)
                .First()
                .GetSyntaxRootAsync();

            var actualTree = resultNode!.ToFullString();

            // Assert
            Assert.True(succeeded);
            Assert.Equal(expectedTree, actualTree);
        }
    }
}