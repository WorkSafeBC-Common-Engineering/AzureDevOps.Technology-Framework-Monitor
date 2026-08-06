using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    public class PythonVersionParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersion";
        private const string majorVersionKey = "PythonMajorVersion";
        private const string inconsistentVersionKey = "PythonInconsistentVersion";

        #endregion

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
                SelectVersionExpression(versionExpressions),
                PythonCommon.HasInconsistentVersions(versionExpressions, ExtractVersion));
        }

        #endregion

        #region Private Methods

        private static string SelectVersionExpression(IReadOnlyList<string> versionExpressions)
        {
            return versionExpressions.Count == 0 ? string.Empty : versionExpressions[0];
        }

        private static bool TryGetVersionExpression(string line, out string versionExpression)
        {
            versionExpression = string.Empty;

            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                return false;
            }

            var match = Regex.Match(trimmedLine, @"\d+(?:\.\d+){0,2}");
            if (!match.Success)
            {
                return false;
            }

            versionExpression = match.Value;
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
            if (!file.Path.Contains(".python-version", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var version = ExtractVersion(versionExpression);
            if (string.IsNullOrWhiteSpace(version))
            {
                return;
            }

            file.AddProperty(versionKey, version);

            if (hasInconsistentVersions)
            {
                file.AddProperty(inconsistentVersionKey, bool.TrueString.ToLowerInvariant());
            }

            file.AddProperty(majorVersionKey, version);
        }

        #endregion
    }
}