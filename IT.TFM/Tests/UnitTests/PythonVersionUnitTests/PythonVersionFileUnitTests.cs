using Parser.Interfaces;

using ProjectData;

using PythonFileParser;

namespace PythonVersionUnitTests
{
    public class PythonVersionFileUnitTests
    {
        private const string VersionPropertyKey = "PythonVersion";
        private const string MajorVersionPropertyKey = "PythonMajorVersion";
        private const string InconsistentVersionPropertyKey = "PythonInconsistentVersion";

        [Fact]
        public void Parse_WhenFileContainsPatchVersion_NormalizesToMajorMinor()
        {
            var file = CreateFileItem();

            Parse(file, "3.12.4");

            AssertVersion(file, "3.12");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFileContainsMajorMinorVersion_ParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file, "3.11");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFileContainsMajorVersionOnly_ParsesMajorVersion()
        {
            var file = CreateFileItem();

            Parse(file, "3");

            AssertVersion(file, "3");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFileContainsMultipleDifferentVersions_UsesHighestVersionAndSetsInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "3.12.4",
                "3.11.9");

            AssertVersion(file, "3.12");
            Assert.Equal("true", file.Properties[InconsistentVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenFileContainsMultipleEquivalentVersions_DoesNotSetInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "3.11.9",
                "3.11");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenNamedEnvironmentContainsEmbeddedVersion_ParsesEmbeddedVersion()
        {
            var file = CreateFileItem();

            Parse(file, "venv-3.11");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenNamedEnvironmentDoesNotContainVersion_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file, "my-project-env");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenFileContainsWhitespace_IgnoresWhitespaceAndParsesValidVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "   ",
                "  3.10.8  ",
                "");

            AssertVersion(file, "3.10");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        private static FileItem CreateFileItem()
        {
            return new FileItem
            {
                FileType = FileItemType.PythonVersion,
                Id = "python-version-test-id",
                Path = ".python-version",
                Url = "https://example/.python-version",
                CommitId = "python-version-test-commit"
            };
        }

        private static void AssertVersion(FileItem file, string expectedVersion)
        {
            Assert.Equal(expectedVersion, file.Properties[VersionPropertyKey]);
            Assert.Equal(expectedVersion, file.Properties[MajorVersionPropertyKey]);
        }

        private static void AssertInconsistentVersionFlagIsNotSet(FileItem file)
        {
            Assert.False(file.Properties.ContainsKey(InconsistentVersionPropertyKey));
        }

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PythonVersionParser();
            parser.Parse(file, content);
        }
    }
}
