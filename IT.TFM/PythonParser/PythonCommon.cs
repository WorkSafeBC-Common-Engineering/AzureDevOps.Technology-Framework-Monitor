using Storage;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    internal class PythonCommon
    {
        #region Private Members

        private static readonly Dictionary<string, PythonVersion> _pythonVersions = [];

        #endregion

        #region Public Methods

        public static PythonVersion? GetPythonVersion(string versionExpression)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return null;
            }

            LoadPythonVersions();

            var normalizedExpression = versionExpression.Trim();

            foreach (var pythonVersion in _pythonVersions.Values
                                                       .Select(v => new { PythonVersion = v, ParsedVersion = ParseVersion(v.Version) })
                                                       .OrderByDescending(v => v.ParsedVersion))
            {
                if (pythonVersion.ParsedVersion == null)
                {
                    continue;
                }

                if (MatchesExpression(pythonVersion.ParsedVersion, normalizedExpression))
                {
                    return pythonVersion.PythonVersion;
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static void LoadPythonVersions()
        {
            if (_pythonVersions.Count > 0)
            {
                return;
            }

            var reader = StorageFactory.GetStorageReader();
            var versions = reader.GetEolVersions()
                                 .Where(v => v.Version.StartsWith("python", StringComparison.OrdinalIgnoreCase));

            foreach (var v in versions)
            {
                _pythonVersions.Add(v.Version, new PythonVersion { Version = v.Version, EolDate = v.EolDate });
            }
        }

        private static bool MatchesExpression(Version candidateVersion, string versionExpression)
        {
            var matches = Regex.Matches(versionExpression, @"(?<operator><=|>=|<|>|==|~=|\^)?\s*(?<version>\d+(?:\.\d+){0,2})");
            if (matches.Count == 0)
            {
                return false;
            }

            foreach (Match match in matches)
            {
                var versionToken = match.Groups["version"].Value;
                var constraintVersion = ParseVersion(versionToken);
                if (constraintVersion == null)
                {
                    return false;
                }

                var constraintOperator = match.Groups["operator"].Value;
                if (!MatchesConstraint(candidateVersion, constraintVersion, constraintOperator, versionToken))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MatchesConstraint(Version candidateVersion, Version constraintVersion, string constraintOperator, string versionToken)
        {
            var comparison = candidateVersion.CompareTo(constraintVersion);

            return constraintOperator switch
            {
                "" or "==" => comparison == 0,
                ">=" => comparison >= 0,
                ">" => comparison > 0,
                "<=" => comparison <= 0,
                "<" => comparison < 0,
                "~=" => comparison >= 0 && candidateVersion < GetCompatibleUpperBound(versionToken),
                "^" => comparison >= 0 && candidateVersion < GetCaretUpperBound(versionToken),
                _ => false
            };
        }

        private static Version GetCompatibleUpperBound(string versionToken)
        {
            var versionParts = ParseVersionParts(versionToken);
            if (versionParts.Count == 0)
            {
                return new Version(0, 0, 0);
            }

            var upperBoundIndex = versionParts.Count <= 2 ? 0 : versionParts.Count - 2;
            versionParts[upperBoundIndex]++;

            for (var i = upperBoundIndex + 1; i < versionParts.Count; i++)
            {
                versionParts[i] = 0;
            }

            return BuildVersion(versionParts);
        }

        private static Version GetCaretUpperBound(string versionToken)
        {
            var versionParts = ParseVersionParts(versionToken);
            if (versionParts.Count == 0)
            {
                return new Version(0, 0, 0);
            }

            var upperBoundIndex = versionParts.FindIndex(part => part != 0);
            upperBoundIndex = upperBoundIndex < 0 ? versionParts.Count - 1 : upperBoundIndex;
            versionParts[upperBoundIndex]++;

            for (var i = upperBoundIndex + 1; i < versionParts.Count; i++)
            {
                versionParts[i] = 0;
            }

            return BuildVersion(versionParts);
        }

        private static Version? ParseVersion(string versionValue)
        {
            var match = Regex.Match(versionValue, @"\d+(?:\.\d+){0,2}");
            if (!match.Success)
            {
                return null;
            }

            var versionParts = ParseVersionParts(match.Value);
            return versionParts.Count == 0 ? null : BuildVersion(versionParts);
        }

        private static List<int> ParseVersionParts(string versionValue)
        {
            var versionParts = new List<int>();
            foreach (var part in versionValue.Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!int.TryParse(part, out var parsedPart))
                {
                    return [];
                }

                versionParts.Add(parsedPart);
            }

            return versionParts;
        }

        private static Version BuildVersion(IReadOnlyList<int> versionParts)
        {
            var major = versionParts.Count > 0 ? versionParts[0] : 0;
            var minor = versionParts.Count > 1 ? versionParts[1] : 0;
            var build = versionParts.Count > 2 ? versionParts[2] : 0;

            return new Version(major, minor, build);
        }

        #endregion
    }
}
