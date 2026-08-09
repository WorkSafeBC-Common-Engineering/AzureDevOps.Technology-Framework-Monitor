using Parser.Interfaces;

using ProjectData;

using PythonFileParser;

namespace PythonVersionUnitTests
{
    public class PythonVersionDockerfileUnitTests
    {
        private const string VersionPropertyKey = "PythonVersionDockerfile";
        private const string MajorVersionPropertyKey = "PythonMajorVersionDockerfile";
        private const string InconsistentVersionPropertyKey = "PythonInconsistentVersion";

        [Fact]
        public void Parse_WhenFromPythonHasPlainVersion_AddsVersionAndMajorVersionProperties()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3.11");

            Assert.Equal("3.11", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.11", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFromPythonHasTagSuffixAndAlias_ParsesCorrectVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3.12-slim AS base");

            AssertVersion(file, "3.12");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFromPythonHasBookwormVariant_ParsesMajorMinorVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3.12-bookworm");

            AssertVersion(file, "3.12");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFromPythonUsesUppercaseAndComment_ParsesCorrectVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM PYTHON:3.10 # pinned runtime");

            Assert.Equal("3.10", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.10", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenMultiplePythonImagesFound_UsesHighestVersionAndSetsInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "FROM python:3.12-slim AS build",
                "FROM alpine:3.20",
                "FROM python:3.10 AS runtime");

            Assert.Equal("3.12", file.Properties[VersionPropertyKey]);
            Assert.Equal("3.12", file.Properties[MajorVersionPropertyKey]);
            AssertInconsistentVersionFlagIsSet(file);
        }

        [Fact]
        public void Parse_WhenMultiplePythonImagesResolveToSameVersion_DoesNotSetInconsistentFlag()
        {
            var file = CreateFileItem();

            Parse(file,
                "FROM python:3.11-slim",
                "FROM python:3.11-alpine");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFromPythonHasPatchVersion_ParsesMajorMinorVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3.11.9");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenFromPythonHasPatchVersionAndVariant_ParsesMajorMinorVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3.10.14-alpine");

            AssertVersion(file, "3.10");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenArgBasedVersionIsUsed_ResolvesVariableAndParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "ARG PYTHON_VERSION=3.12",
                "FROM python:${PYTHON_VERSION}");

            AssertVersion(file, "3.12");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenArgBasedPatchVersionWithSuffixIsUsed_ResolvesAndParsesMajorMinorVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "ARG PYTHON_VERSION=3.11.8",
                "FROM python:${PYTHON_VERSION}-slim");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenDollarArgSyntaxIsUsed_ResolvesAndParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file,
                "ARG PYTHON_VERSION=3.10",
                "FROM python:$PYTHON_VERSION");

            AssertVersion(file, "3.10");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPythonTagUsesMajorOnly_ParsesMajorVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3");

            AssertVersion(file, "3");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenNonOfficialPythonImageContainsVersion_ParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM mcr.microsoft.com/devcontainers/python:3.12");

            AssertVersion(file, "3.12");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenPythonRuntimeImageContainsVersion_ParsesVersion()
        {
            var file = CreateFileItem();

            Parse(file, "FROM ghcr.io/company/python-runtime:3.11");

            AssertVersion(file, "3.11");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        [Fact]
        public void Parse_WhenNoPythonBaseImageExists_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "FROM node:20",
                "RUN echo done");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenPythonTagHasNoNumericVersion_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:latest");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenPythonImageHasNoTag_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenPythonIsInstalledInRunCommandOnly_DoesNotAddProperties()
        {
            var file = CreateFileItem();

            Parse(file,
                "FROM ubuntu:24.04",
                "RUN apt-get install -y python3.11");

            Assert.Empty(file.Properties);
        }

        [Fact]
        public void Parse_WhenFromPythonHasMinorVersionEndingInZero_PreservesTrailingZero()
        {
            var file = CreateFileItem();

            Parse(file, "FROM python:3.10");

            AssertVersion(file, "3.10");
            AssertInconsistentVersionFlagIsNotSet(file);
        }

        private static FileItem CreateFileItem()
        {
            return new FileItem
            {
                FileType = FileItemType.PythonDockerfile,
                Id = "docker-test-id",
                Path = "Dockerfile",
                Url = "https://example/Dockerfile",
                CommitId = "docker-test-commit"
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

        private static void AssertVersion(FileItem file, string expectedVersion)
        {
            Assert.Equal(expectedVersion, file.Properties[VersionPropertyKey]);
            Assert.Equal(expectedVersion, file.Properties[MajorVersionPropertyKey]);
        }

        private static void Parse(FileItem file, params string[] content)
        {
            var parser = (IFileParser)new PythonDockerfileParser();
            parser.Parse(file, content);
        }
    }
}
