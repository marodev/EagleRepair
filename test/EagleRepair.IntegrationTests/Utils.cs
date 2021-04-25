using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace EagleRepair.IntegrationTests
{
    public static class Utils
    {
        public static Solution Workspace(string sourceCode, string projectName = "NewProject", string fileName = "NewFile.cs")
        {
            var workspace = new AdhocWorkspace();
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, projectName, projectName, LanguageNames.CSharp);
            var newProject = workspace.AddProject(projectInfo);
            var sourceText = SourceText.From(sourceCode);
            var newDocument = workspace.AddDocument(newProject.Id, fileName, sourceText);

            return newDocument.Project.Solution;
        }
    }
}