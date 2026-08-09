using Parser.Interfaces;

using ProjectData;

using PythonFileParser;

namespace PythonVersionUnitTests
{
    public class PythonVersionSetupPyUnitTests
    {
        private const string VersionPropertyKey = "PythonVersionSetupPy";
        private const string MajorVersionPropertyKey = "PythonMajorVersionSetupPy";
        private const string InconsistentVersionPropertyKey = "PythonInconsistentVersion";

        static PythonVersionSetupPyUnitTests()
        {
            PythonVersionTestDataSeeder.SeedPythonVersions();
        }

        [Fact]
        public void Parse_WhenPythonRequiresIsStringLiteralMinimumVersion_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "from setuptools import setup",
                "setup(",
                "    name=\"myproject\",",
                "    python_requires=\">=3.10\"",
                ")");

            Assert.Equal(">=3.10", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.14", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPythonRequiresIsVersionRange_ParsesEntireConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "setup(",
                "    python_requires=\">=3.10,<4\"",
                ")");

            Assert.Equal(">=3.10,<4", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.14", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresIsExactVersion_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "setup(",
                "    python_requires=\"==3.11\"",
                ")");

            Assert.Equal("==3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresUsesWildcard_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "setup(",
                "    python_requires=\"==3.11.*\"",
                ")");

            Assert.Equal("==3.11.*", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresUsesSingleQuotes_ParsesConstraint()
        {
            var file = CreateFileItem();

            Parse(file,
                "setup(",
                "    python_requires='>=3.11'",
                ")");

            Assert.Equal(">=3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.14", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonRequiresUsesVariableValue_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "PYTHON_REQUIRES = \">=3.11\"",
                "setup(",
                "    python_requires=PYTHON_REQUIRES",
                ")");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenOnlyPackageVersionExists_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "setup(",
                "    version=\"1.2.3\"",
                ")");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenPythonRequiresAppearsOutsideSetupCall_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "python_requires = \">=3.11\"",
                "name = \"example\"");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenMultiplePythonRequiresArgumentsExist_SetsInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "setup(",
                "    python_requires=\">=3.10\",",
                "    python_requires=\"==3.11\"",
                ")");

            Assert.Equal("true", file.Properties[InconsistentVersionPropertyKey]);
        }

        private static FileItem CreateFileItem()
        {
            return new FileItem
            {
                FileType = FileItemType.PythonSetupPy,
                Id = "setuppy-test-id",
                Path = "setup.py",
                Url = "https://example/setup.py",
                CommitId = "setuppy-test-commit"
            };
        }

        private static void AssertInconsistentVersionFlagIsNotSet(FileItem file)
        {
            Assert.False(file.Properties.ContainsKey(InconsistentVersionPropertyKey));
        }

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PythonSetupPyParser();
            parser.Parse(file, content);
        }
    }
}
