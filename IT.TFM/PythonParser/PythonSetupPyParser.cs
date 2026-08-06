using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    public class PythonSetupPyParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersionSetupPy";
        private const string majorVersionKey = "PythonMajorVersionSetupPy";
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
            var insideSetupCall = false;
            var setupCallDepth = 0;

            for (var i = 0; i < content.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(content[i]))
                {
                    continue;
                }

                var line = content[i];

                if (!insideSetupCall && TryEnterSetupCall(line, ref setupCallDepth))
                {
                    insideSetupCall = true;
                }

                if (insideSetupCall && TryGetVersionExpression(line, out var versionExpression))
                {
                    versionExpressions.Add(versionExpression);
                }

                if (insideSetupCall)
                {
                    setupCallDepth += CountOccurrences(line, '(') - CountOccurrences(line, ')');
                    if (setupCallDepth <= 0)
                    {
                        insideSetupCall = false;
                        setupCallDepth = 0;
                    }
                }
            }

            ParseVersionFile(
                file,
                PythonCommon.SelectHighestVersionExpression(versionExpressions, PythonCommon.ResolveVersionExpression),
                PythonCommon.HasInconsistentVersions(versionExpressions, PythonCommon.ResolveVersionExpression));
        }

        #endregion

        #region Private Methods

        private static bool TryEnterSetupCall(string line, ref int setupCallDepth)
        {
            var setupMatch = Regex.Match(line, @"\bsetup\s*\(", RegexOptions.IgnoreCase);
            if (!setupMatch.Success)
            {
                return false;
            }

            setupCallDepth = CountOccurrences(line, '(') - CountOccurrences(line, ')');
            return true;
        }

        private static bool TryGetVersionExpression(string line, out string versionExpression)
        {
            versionExpression = string.Empty;

            var match = Regex.Match(line, @"\bpython_requires\s*=\s*([""'])(?<value>.*?)\1", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return false;
            }

            var rawValue = match.Groups["value"].Value.Trim();
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            versionExpression = rawValue;
            return true;
        }

        private static void ParseVersionFile(FileItem file, string versionExpression, bool hasInconsistentVersions)
        {
            if (!file.Path.Contains("setup.py", StringComparison.OrdinalIgnoreCase))
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

        private static int CountOccurrences(string value, char character)
        {
            var count = 0;

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == character)
                {
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}