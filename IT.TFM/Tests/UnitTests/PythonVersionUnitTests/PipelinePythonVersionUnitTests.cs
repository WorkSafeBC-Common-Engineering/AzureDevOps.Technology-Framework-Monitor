using Parser.Interfaces;

using ProjectData;

using YamlFileParser;

namespace PythonVersionUnitTests
{
    public class PipelinePythonVersionUnitTests
    {
        private const string TaskDetectedPropertyKey = "UsesPythonVersionTask";
        private const string VersionPropertyKey = "PythonVersion";
        private const string MajorVersionPropertyKey = "PythonMajorVersion";
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
        public void Parse_WhenPipelineUsesListFormatVariable_ResolvesVariableValue()
        {
            var file = CreateFileItem();

            Parse(file,
                "variables:",
                "- name: pythonVersion",
                "  value: 3.11",
                "steps:",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: $(pythonVersion)");

            Assert.Equal("true", file.Properties[TaskDetectedPropertyKey]);
            Assert.Equal("3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPipelineUsesTemplateExpressionVariable_ResolvesVariableValue()
        {
            var file = CreateFileItem();

            Parse(file,
                "variables:",
                "  pythonVersion: 3.10",
                "steps:",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: ${{variables.pythonVersion}}");

            Assert.Equal("true", file.Properties[TaskDetectedPropertyKey]);
            Assert.Equal("3.10", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.10", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenVersionSpecReferencesExternalVariable_AddsDetectionPropertyOnly()
        {
            var file = CreateFileItem();

            // Variable is not defined in the pipeline file (e.g. from a variable group)
            Parse(file,
                "steps:",
                "- task: UsePythonVersion@0",
                "  inputs:",
                "    versionSpec: $(pythonVersionFromVariableGroup)");

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
