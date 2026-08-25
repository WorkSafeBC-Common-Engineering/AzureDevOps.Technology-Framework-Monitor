using Parser.Interfaces;

using ProjectData;

using PythonFileParser;

namespace PythonVersionUnitTests
{
    public class PythonVersionPyProjectTomlUnitTests
    {
        private const string VersionPropertyKey = "PythonVersion";
        private const string MajorVersionPropertyKey = "PythonMajorVersion";
        private const string InconsistentVersionPropertyKey = "PythonInconsistentVersion";

        static PythonVersionPyProjectTomlUnitTests()
        {
            PythonVersionTestDataSeeder.SeedPythonVersions();
        }

        [Fact]
        public void Parse_WhenRequiresPythonIsWithinProjectSection_AddsVersionAndMajorVersionProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "",
                "requires-python = \"3.11\" # inline comment");

            Assert.Equal("3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenRequiresPythonIsOutsideProjectSection_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[tool.poetry.dependencies]",
                "requires-python = \"3.9\"",
                "[build-system]",
                "requires-python = \"3.12\"");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenMultipleValidVersionsExist_UsesHighestVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "[tool.poetry.dependencies]",
                "python = \"3.12\"",
                "[project]",
                "requires-python = \"3.9\"");

            Assert.Equal("3.12", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.12", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsSet(file);
        }

        [Fact]
        public void Parse_WhenPythonKeyIsWithinPoetryDependenciesSectionInDifferentCasing_ParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "[tool.poetry.dependencies]",
                "PYTHON = '3.10'");

            Assert.Equal("3.10", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.10", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPythonIsOutsidePoetryDependenciesSection_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "python = \">=3.10,<3.12\"",
                "[tool.poetry]",
                "python_version = \"3.11\"");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenPythonVersionIsWithinPoetryDependenciesSection_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[tool.poetry.dependencies]",
                "python_version = \"3.11\"");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenRequiresPythonUsesRangeWithinProjectSection_ResolvesHighestMatchingVersionFromMockedReader()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "requires-python = \">=3.10,<3.12\"");

            Assert.Equal(">=3.10,<3.12", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenMultipleProjectVersionsExist_UsesHighestVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "requires-python = \"3.12\"",
                "requires-python = \"3.10\"");

            Assert.Equal("3.12", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.12", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsSet(file);
        }

        [Fact]
        public void Parse_WhenMultipleValidEntriesResolveToSameVersion_DoesNotSetInconsistentVersionProperty()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "requires-python = \">=3.10,<3.12\"",
                "[tool.poetry.dependencies]",
                "python = \"3.11\"");

            Assert.Equal(">=3.10,<3.12", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenRequiresPythonWithinProjectSectionHasNoNumericVersion_AddsOnlyRawVersionProperty()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "requires-python = \"latest\"");

            Assert.Equal("latest", file.Properties[VersionPropertyKey]);
            Assert.False(file.Properties.ContainsKey(MajorVersionPropertyKey));
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenNoSupportedVersionKeyExists_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "name = \"demo\"",
                "version = \"1.0.0\"");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenVersionValueIsEmpty_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "requires-python = \"\"");

            Assert.Empty(file.Properties);
        }

        private static FileItem CreateFileItem()
        {
            return new FileItem
            {
                FileType = FileItemType.PythonProjectToml,
                Id = "test-id",
                Path = "pyproject.toml",
                Url = "https://example/pyproject.toml",
                CommitId = "test-commit"
            };
        }

        private static void AssertInconsistentVersionFlagIsSet(FileItem file)
        {
            Assert.Equal("true", file.Properties[InconsistentVersionPropertyKey]);
        }

        private static void AssertInconsistentVersionFlagIsNotSet(FileItem file)
        {
            Assert.False(file.Properties.ContainsKey(InconsistentVersionPropertyKey));
        }

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PythonProjectTomlParser();
            parser.Parse(file, content);
        }
    }
}
