using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    public class PythonSetupCfgParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersionSetupCfg";
        private const string majorVersionKey = "PythonMajorVersionSetupCfg";
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

                if (currentSection.Equals("options", StringComparison.OrdinalIgnoreCase)
                    && TryGetVersionExpression(content[i], out var versionExpression))
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

        private static bool TryGetVersionExpression(string line, out string versionExpression)
        {
            versionExpression = string.Empty;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                return false;
            }

            var key = line[..separatorIndex].Trim();
            if (!key.Equals("python_requires", StringComparison.OrdinalIgnoreCase))
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

        private static string ExtractVersion(string versionExpression)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return string.Empty;
            }

            var match = Regex.Match(versionExpression, @"\d+(?:\.\d+){0,2}");
            if (!match.Success)
            {
                return string.Empty;
            }

            var versionParts = match.Value.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (versionParts.Length <= 1)
            {
                return versionParts[0];
            }

            return $"{versionParts[0]}.{versionParts[1]}";
        }

        private static void ParseVersionFile(FileItem file, string versionExpression, bool hasInconsistentVersions)
        {
            if (!file.Path.Contains("setup.cfg", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

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

            file.AddProperty(majorVersionKey, version);
        }

        #endregion
    }
}