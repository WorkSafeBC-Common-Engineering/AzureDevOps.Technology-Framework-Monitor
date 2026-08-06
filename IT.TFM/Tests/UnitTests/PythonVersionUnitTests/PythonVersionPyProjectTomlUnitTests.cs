using Parser.Interfaces;

using ProjectData;
using ProjectData.Interfaces;

using PythonFileParser;

using Moq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PythonVersionUnitTests
{
    public class PythonVersionPyProjectTomlUnitTests
    {
        private const string VersionPropertyKey = "PythonVersionPyProjectToml";
        private const string MajorVersionPropertyKey = "PythonVersion";

        static PythonVersionPyProjectTomlUnitTests()
        {
            SeedPythonVersionsFromMockedReader();
        }

        [Fact]
        public void Parse_WhenRequiresPythonHasPlainVersion_AddsVersionAndMajorVersionProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "[project]",
                "",
                "requires-python = \"3.11\" # inline comment");

            Assert.Equal("3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenPythonVersionKeyComesFirst_UsesFirstMatchingVersionOnly()
        {
            var file = CreateFileItem();

            Parse(file,
                "python_version = \"3.9\"",
                "requires-python = \"3.12\"");

            Assert.Equal("3.9", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.9", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenKeyIsPythonInDifferentCasing_ParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file, "PYTHON = '3.10'");

            Assert.Equal("3.10", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.10", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenRequiresPythonUsesRange_ResolvesHighestMatchingVersionFromMockedReader()
        {
            var file = CreateFileItem();

            Parse(file, "requires-python = \">=3.10,<3.12\"");

            Assert.Equal(">=3.10,<3.12", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
        }

        [Fact]
        public void Parse_WhenVersionExpressionHasNoNumericVersion_AddsOnlyRawVersionProperty()
        {
            var file = CreateFileItem();

            Parse(file, "requires-python = \"latest\"");

            Assert.Equal("latest", file.Properties[VersionPropertyKey]);
            Assert.False(file.Properties.ContainsKey(MajorVersionPropertyKey));
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

            Parse(file, "requires-python = \"\"");

            Assert.Empty(file.Properties);
        }

        private static void SeedPythonVersionsFromMockedReader()
        {
            var mockReader = new Mock<IStorageReader>();
            mockReader.Setup(reader => reader.GetEolVersions()).Returns(new List<EolVersion>
            {
                new() { Version = "python 3.9", EolDate = new DateOnly(2025, 10, 31) },
                new() { Version = "python 3.10", EolDate = new DateOnly(2026, 10, 31) },
                new() { Version = "python 3.11", EolDate = new DateOnly(2027, 10, 31) },
                new() { Version = "python 3.12", EolDate = new DateOnly(2028, 10, 31) }
            });

            PopulatePythonCommonCache(mockReader.Object);
        }

        private static void PopulatePythonCommonCache(IStorageReader storageReader)
        {
            var assembly = typeof(PythonProjectTomlParser).Assembly;
            var pythonCommonType = assembly.GetType("PythonFileParser.PythonCommon")
                ?? throw new InvalidOperationException("Unable to find PythonCommon type.");
            var pythonVersionType = assembly.GetType("PythonFileParser.PythonVersion")
                ?? throw new InvalidOperationException("Unable to find PythonVersion type.");

            var pythonVersionsField = pythonCommonType.GetField("_pythonVersions", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("Unable to find PythonCommon._pythonVersions field.");

            if (pythonVersionsField.GetValue(null) is not IDictionary pythonVersionsDictionary)
            {
                throw new InvalidOperationException("Unable to access PythonCommon._pythonVersions dictionary.");
            }

            pythonVersionsDictionary.Clear();

            var versionProperty = pythonVersionType.GetProperty("Version")
                ?? throw new InvalidOperationException("Unable to find PythonVersion.Version property.");
            var eolDateProperty = pythonVersionType.GetProperty("EolDate")
                ?? throw new InvalidOperationException("Unable to find PythonVersion.EolDate property.");

            foreach (var eolVersion in storageReader.GetEolVersions().Where(v => v.Version.StartsWith("python", StringComparison.OrdinalIgnoreCase)))
            {
                var pythonVersionInstance = Activator.CreateInstance(pythonVersionType)
                    ?? throw new InvalidOperationException("Unable to create PythonVersion instance.");

                versionProperty.SetValue(pythonVersionInstance, eolVersion.Version);
                eolDateProperty.SetValue(pythonVersionInstance, eolVersion.EolDate);

                pythonVersionsDictionary.Add(eolVersion.Version, pythonVersionInstance);
            }
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

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PythonProjectTomlParser();
            parser.Parse(file, content);
        }
    }
}
