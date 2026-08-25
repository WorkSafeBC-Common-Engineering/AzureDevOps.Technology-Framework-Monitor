using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;

namespace PythonFileParser
{
    public class PythonVersionParser : IFileParser
    {
        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var versionExpressions = new List<string>();

            for (var i = 0; i < content.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(content[i]))
                {
                    continue;
                }

                if (TryGetVersionExpression(content[i], out var versionExpression))
                {
                    versionExpressions.Add(versionExpression);
                }
            }

            ParseVersionFile(
                file,
                PythonCommon.SelectHighestVersionExpression(versionExpressions, PythonCommon.ExtractNormalizedVersion),
                PythonCommon.HasInconsistentVersions(versionExpressions, PythonCommon.ExtractNormalizedVersion));
        }

        #endregion

        #region Private Methods

        private static bool TryGetVersionExpression(string line, out string versionExpression)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                versionExpression = string.Empty;
                return false;
            }

            return PythonCommon.TryExtractVersionToken(trimmedLine, out versionExpression);
        }

        private static void ParseVersionFile(FileItem file, string versionExpression, bool hasInconsistentVersions)
        {
            if (!file.Path.Contains(".python-version", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            PythonCommon.AddVersionProperties(
                file,
                versionExpression,
                PythonCommon.ExtractNormalizedVersion,
                hasInconsistentVersions,
                storeExtractedInVersionKey: true);
        }

        #endregion
    }
}