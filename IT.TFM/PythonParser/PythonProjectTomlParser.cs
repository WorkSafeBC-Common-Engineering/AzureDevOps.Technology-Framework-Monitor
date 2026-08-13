using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;

namespace PythonFileParser
{
    public class PythonProjectTomlParser : IFileParser
    {
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

                if (PythonCommon.TryGetSectionName(content[i], out var sectionName))
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
                PythonCommon.SelectHighestVersionExpression(versionExpressions, ExtractVersion),
                PythonCommon.HasInconsistentVersions(versionExpressions, ExtractVersion));
        }

        #endregion

        #region Private Methods

        private static bool TryGetVersionExpression(string currentSection, string line, out string versionExpression)
        {
            versionExpression = string.Empty;

            if (!PythonCommon.TryParseAssignment(line, out var key, out var value))
            {
                return false;
            }

            if (!IsVersionKeyForSection(currentSection, key))
            {
                return false;
            }

            versionExpression = value;
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
            return PythonCommon.ResolveVersionExpression(versionExpression);
        }

        private static void ParseVersionFile(FileItem file, string versionExpression, bool hasInconsistentVersions)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return;
            }

            PythonCommon.AddVersionProperties(
                file,
                versionExpression,
                ExtractVersion,
                hasInconsistentVersions);
        }

        #endregion
    }
}
