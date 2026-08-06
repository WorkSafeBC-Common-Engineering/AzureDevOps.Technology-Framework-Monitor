using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    public class PythonProjectTomlParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersionPyProjectToml";
        private const string majorVersionKey = "PythonVersion";
        private const string inconsistentVersionKey = "PythonInconsistentVersion";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var currentSection = string.Empty;
            var versionExpressions = new List<string>();

            for (var i = 0; i < content.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(content[i]))
                {
                    continue;
                }

                if (TryGetSectionName(content[i], out var sectionName))
                {
                    currentSection = sectionName;
                    continue;
                }

                if (TryGetVersionExpression(currentSection, content[i], out var versionExpression))
                {
                    versionExpressions.Add(versionExpression);
                }
            }

            ParseVersionFile(
                file,
                PythonCommon.SelectLowestVersionExpression(versionExpressions, ExtractVersion),
                PythonCommon.HasInconsistentVersions(versionExpressions, ExtractVersion));
        }

        #endregion

        #region Private Methods

        private static bool TryGetSectionName(string line, out string sectionName)
        {
            sectionName = string.Empty;

            var trimmedLine = line.Trim();
            if (!trimmedLine.StartsWith('[') || !trimmedLine.EndsWith(']'))
            {
                return false;
            }

            sectionName = trimmedLine[1..^1].Trim();
            return !string.IsNullOrWhiteSpace(sectionName);
        }

        private static bool TryGetVersionExpression(string currentSection, string line, out string versionExpression)
        {
            versionExpression = string.Empty;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                return false;
            }

            var key = line[..separatorIndex].Trim();
            if (!IsVersionKeyForSection(currentSection, key))
            {
                return false;
            }

            var rawValue = line[(separatorIndex + 1)..].Trim();

            var commentIndex = rawValue.IndexOf('#');
            if (commentIndex >= 0)
            {
                rawValue = rawValue[..commentIndex].Trim();
            }

            rawValue = rawValue.Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            versionExpression = rawValue;
            return true;
        }

        private static bool IsVersionKeyForSection(string currentSection, string key)
        {
            if (currentSection.Equals("project", StringComparison.OrdinalIgnoreCase))
            {
                return key.Equals("requires-python", StringComparison.OrdinalIgnoreCase);
            }

            if (currentSection.Equals("tool.poetry.dependencies", StringComparison.OrdinalIgnoreCase))
            {
                return key.Equals("python", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string ExtractVersion(string versionExpression)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return string.Empty;
            }

            var matches = Regex.Matches(versionExpression, @"(?<operator><=|>=|<|>|==|~=|\^)?\s*(?<version>\d+(?:\.\d+){0,2})");
            if (matches.Count == 0)
            {
                return string.Empty;
            }

            if (matches.Count == 1)
            {
                var versionToken = matches[0].Groups["version"].Value;
                var constraintOperator = matches[0].Groups["operator"].Value;

                if (constraintOperator is "" or "==")
                {
                    return versionToken;
                }
            }

            var pythonVersion = PythonCommon.GetPythonVersion(versionExpression);
            return pythonVersion == null ? string.Empty : TrimPythonPrefix(pythonVersion.Version);
        }

        private static string TrimPythonPrefix(string version)
        {
            const string prefix = "python ";

            return version.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? version[prefix.Length..].Trim()
                : version.Trim();
        }

        private static void ParseVersionFile(FileItem file, string versionExpression, bool hasInconsistentVersions)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return;
            }

            file.AddProperty(versionKey, versionExpression);

            if (hasInconsistentVersions)
            {
                file.AddProperty(inconsistentVersionKey, bool.TrueString.ToLowerInvariant());
            }

            var version = ExtractVersion(versionExpression);
            if (string.IsNullOrWhiteSpace(version))
            {
                return;
            }

            //This covers versions that have the '-slim', or other suffixes
            version = version.Contains('-') ? version.Split("-")[0] : version;

            file.AddProperty(majorVersionKey, version);
        }

        #endregion
    }
}
