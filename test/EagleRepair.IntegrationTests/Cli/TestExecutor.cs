using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using EagleRepair.Ast.Parser;
using EagleRepair.Cli;
using EagleRepair.IntegrationTests.Mock;
using Microsoft.CodeAnalysis;
using Xunit;

namespace EagleRepair.IntegrationTests.Cli
{
    public static class TestExecutor
    {
        public static async Task Run(string inputTree, string expectedTree)
        {
            // Arrange
            var diBuilder = DiContainerTestConfig.Builder();
            var solutionParserMock = new MockSolutionParser(Utils.Workspace(inputTree));
            diBuilder.Register(c => solutionParserMock).As<ISolutionParser>();
            var diContainer = diBuilder.Build();
            await using var scope = diContainer.BeginLifetimeScope();
            var app = scope.Resolve<IApplication>();
            var cmdArgs = new List<string> {"-p", "ignore.sln"};

            // Act
            var succeeded = await app.Run(cmdArgs);
            var resultNode = await ExtractRootAsync(solutionParserMock);
            var actualTree = resultNode!.ToFullString();

            // Assert
            Assert.True(succeeded);

            expectedTree = UnifyNewLineCharacters(expectedTree);
            actualTree = UnifyNewLineCharacters(actualTree);
            Assert.Equal(expectedTree, actualTree);
        }

        private static string UnifyNewLineCharacters(string text)
        {
            return Regex.Replace(text, @"\r\n?|\n", Environment.NewLine);
        }

        private static async Task<SyntaxNode> ExtractRootAsync(ISolutionParser solutionParser)
        {
            return await solutionParser.Workspace().CurrentSolution.Projects
                .SelectMany(p => p.Documents)
                .First()
                .GetSyntaxRootAsync();
        }
    }
}
