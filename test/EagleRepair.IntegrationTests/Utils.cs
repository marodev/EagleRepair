using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace EagleRepair.IntegrationTests
{
    public static class Utils
    {
        public static Solution Workspace(string sourceCode, string projectName = "NewProject",
            string fileName = "NewFile.cs")
        {
            // if this doesn't work, there is a workaround: https://github.com/jaredpar/basic-reference-assemblies / https://www.nuget.org/packages/Basic.Reference.Assemblies/
            var trustedAssembliesPaths =
                ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var references = trustedAssembliesPaths
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            var workspace = new AdhocWorkspace();
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, projectName, projectName,
                LanguageNames.CSharp, metadataReferences: references);
            var newProject = workspace.AddProject(projectInfo);
            var sourceText = SourceText.From(sourceCode);
            var newDocument = workspace.AddDocument(newProject.Id, fileName, sourceText);

            return newDocument.Project.Solution;
        }
    }
}
