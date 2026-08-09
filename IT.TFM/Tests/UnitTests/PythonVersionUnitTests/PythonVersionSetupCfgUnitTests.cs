using Parser.Interfaces;

using ProjectData;

using PythonFileParser;

namespace PythonVersionUnitTests
{
    public class PythonVersionSetupCfgUnitTests
    {
        private const string VersionPropertyKey = "PythonVersion";
        private const string MajorVersionPropertyKey = "PythonMajorVersion";
        private const string InconsistentVersionPropertyKey = "PythonInconsistentVersion";

        static PythonVersionSetupCfgUnitTests()
        {
            PythonVersionTestDataSeeder.SeedPythonVersions();
        }

        [Fact]
        public void Parse_WhenPythonRequiresIsInOptionsSectionWithMinimumVersion_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "[metadata]",
                "name = sample",
                "[options]",
                "python_requires = >=3.10");

            Assert.Equal(">=3.10", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.14", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPythonRequiresIsVersionRange_ParsesEntireConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "[options]",
                "python_requires = >=3.10,<4");

            Assert.Equal(">=3.10,<4", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.14", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresIsExactVersion_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "[options]",
                "python_requires = ==3.11");

            Assert.Equal("==3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresUsesWildcard_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "[options]",
                "python_requires = ==3.11.*");

            Assert.Equal("==3.11.*", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresContainsExcludedVersions_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "[options]",
                "python_requires = >=3.10,!=3.11.0,<3.14");

            Assert.Equal(">=3.10,!=3.11.0,<3.14", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.13", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresExistsOutsideOptionsSection_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[metadata]",
                "python_requires = >=3.11");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenMultiplePythonRequiresDeclarationsExist_SetsInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "[options]",
                "python_requires = >=3.10",
                "python_requires = ==3.11");

            Assert.Equal("true", file.Properties[InconsistentVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonAppearsOnlyInDependencyList_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[options]",
                "install_requires =",
                "    requests>=2.31.0",
                "    python-dateutil>=2.8.2");

            Assert.Empty(file.Properties);
        }

        private static FileItem CreateFileItem()
        {
            return new FileItem
            {
                FileType = FileItemType.PythonSetupCfg,
                Id = "setupcfg-test-id",
                Path = "setup.cfg",
                Url = "https://example/setup.cfg",
                CommitId = "setupcfg-test-commit"
            };
        }

        private static void AssertInconsistentVersionFlagIsNotSet(FileItem file)
        {
            Assert.False(file.Properties.ContainsKey(InconsistentVersionPropertyKey));
        }

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PythonSetupCfgParser();
            parser.Parse(file, content);
        }
    }
}
