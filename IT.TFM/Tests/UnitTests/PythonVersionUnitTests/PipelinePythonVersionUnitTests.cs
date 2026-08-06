using Parser.Interfaces;

using ProjectData;

using YamlFileParser;

namespace PythonVersionUnitTests
{
    public class PipelinePythonVersionUnitTests
    {
        private const string TaskDetectedPropertyKey = "UsesPythonVersionTask";
        private const string VersionPropertyKey = "PythonVersionPipeline";
        private const string MajorVersionPropertyKey = "PythonMajorVersionPipeline";
        private const string InconsistentVersionPropertyKey = "PythonInconsistentVersion";

        [Fact]
        public void Parse_WhenPipelineUsesPythonVersionTask_AddsPythonVersionProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "steps:",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: '3.11'");

            Assert.Equal("true", file.Properties[TaskDetectedPropertyKey]);
            Assert.Equal("3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPipelineUsesPythonVersionVariable_ResolvesVariableValue()
        {
            var file = CreateFileItem();

            Parse(file,
                "variables:",
                "  pythonVersion: 3.12.4",
                "steps:",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: $(pythonVersion)");

            Assert.Equal("true", file.Properties[TaskDetectedPropertyKey]);
            Assert.Equal("3.12.4", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.12", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPipelineHasMultiplePythonVersionTasks_UsesHighestVersionAndSetsInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "steps:",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: '3.10'",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: '3.11.8'");

            Assert.Equal("true", file.Properties[TaskDetectedPropertyKey]);
            Assert.Equal("3.11.8", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            Assert.Equal("true", file.Properties[InconsistentVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPipelineUsesPythonVersionTaskWithoutVersionSpec_AddsDetectionPropertyOnly()
        {
            var file = CreateFileItem();

            Parse(file,
                "steps:",
                "- task: UsePythonVersion@0",
                "  displayName: Use Python");

            Assert.Equal("true", file.Properties[TaskDetectedPropertyKey]);
            Assert.False(file.Properties.ContainsKey(VersionPropertyKey));
            Assert.False(file.Properties.ContainsKey(MajorVersionPropertyKey));
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPipelineDoesNotUsePythonVersionTask_DoesNotAddPythonProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "steps:",
                "- script: dotnet build");

            Assert.False(file.Properties.ContainsKey(TaskDetectedPropertyKey));
            Assert.False(file.Properties.ContainsKey(VersionPropertyKey));
            Assert.False(file.Properties.ContainsKey(MajorVersionPropertyKey));
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        private static FileItem CreateFileItem()
        {
            return new FileItem
            {
                FileType = FileItemType.YamlPipeline,
                Id = "pipeline-test-id",
                Path = "azure-pipelines.yml",
                Url = "https://example/azure-pipelines.yml",
                CommitId = "pipeline-test-commit"
            };
        }

        private static void AssertInconsistentVersionFlagIsNotSet(FileItem file)
        {
            Assert.False(file.Properties.ContainsKey(InconsistentVersionPropertyKey));
        }

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PipelineParser();
            parser.Parse(file, content);
        }
    }
}
