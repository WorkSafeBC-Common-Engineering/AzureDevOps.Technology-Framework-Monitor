using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;

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

                if (PythonCommon.TryGetSectionName(content[i], out var sectionName))
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
                PythonCommon.SelectHighestVersionExpression(versionExpressions, PythonCommon.ResolveVersionExpression),
                PythonCommon.HasInconsistentVersions(versionExpressions, PythonCommon.ResolveVersionExpression));
        }

        #endregion

        #region Private Methods

        private static bool TryGetVersionExpression(string line, out string versionExpression)
        {
            versionExpression = string.Empty;

            if (!PythonCommon.TryParseAssignment(line, out var key, out var value))
            {
                return false;
            }

            if (!key.Equals("python_requires", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            versionExpression = value;
            return true;
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

            PythonCommon.AddVersionProperties(
                file,
                versionKey,
                majorVersionKey,
                versionExpression,
                PythonCommon.ResolveVersionExpression,
                hasInconsistentVersions,
                inconsistentVersionKey);
        }

        #endregion
    }
}