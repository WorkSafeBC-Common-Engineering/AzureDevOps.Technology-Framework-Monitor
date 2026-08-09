using Storage;

using ProjectData;

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

        internal static string SelectHighestVersionExpression(IReadOnlyList<string> versionExpressions, Func<string, string> extractVersion)
        {
            if (versionExpressions.Count == 0)
            {
                return string.Empty;
            }

            if (versionExpressions.Count == 1)
            {
                return versionExpressions[0];
            }

            var selectedVersion = versionExpressions
                .Select(expression => new
                {
                    Expression = expression,
                    ComparableVersion = TryParseComparableVersion(extractVersion(expression), out var comparableVersion) ? comparableVersion : null
                })
                .Where(item => item.ComparableVersion != null)
                .OrderByDescending(item => item.ComparableVersion)
                .FirstOrDefault();

            return selectedVersion?.Expression ?? versionExpressions[0];
        }

        internal static bool HasInconsistentVersions(IReadOnlyList<string> versionExpressions, Func<string, string> extractVersion)
        {
            if (versionExpressions.Count <= 1)
            {
                return false;
            }

            var distinctVersions = versionExpressions
                .Select(versionExpression => GetNormalizedComparisonVersion(versionExpression, extractVersion))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            return distinctVersions > 1;
        }

        internal static string ExtractNormalizedVersion(string versionExpression)
        {
            if (!TryExtractVersionToken(versionExpression, out var versionToken))
            {
                return string.Empty;
            }

            var versionParts = versionToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (versionParts.Length <= 1)
            {
                return versionParts[0];
            }

            return $"{versionParts[0]}.{versionParts[1]}";
        }

        internal static string ResolveVersionExpression(string versionExpression)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return string.Empty;
            }

            var matchingVersion = GetPythonVersion(versionExpression);
            if (matchingVersion != null)
            {
                return ExtractNormalizedVersionFromRuntimeVersion(matchingVersion.Version);
            }

            return ExtractLowestSpecifiedVersion(versionExpression);
        }

        internal static bool TryExtractVersionToken(string value, out string versionToken)
        {
            versionToken = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var match = Regex.Match(value, @"\d+(?:\.\d+){0,2}");
            if (!match.Success)
            {
                return false;
            }

            versionToken = match.Value;
            return true;
        }

        internal static bool TryGetSectionName(string line, out string sectionName)
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

        internal static bool TryParseAssignment(string line, out string key, out string value)
        {
            key = string.Empty;
            value = string.Empty;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                return false;
            }

            key = line[..separatorIndex].Trim();
            if (string.IsNullOrWhiteSpace(key))
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

            value = rawValue;
            return true;
        }

        internal static void AddVersionProperties(
            FileItem file,
            string versionKey,
            string majorVersionKey,
            string versionExpression,
            Func<string, string> extractVersion,
            bool hasInconsistentVersions,
            string inconsistentVersionKey = "PythonInconsistentVersion",
            bool storeExtractedInVersionKey = false)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
            {
                return;
            }

            var version = extractVersion(versionExpression);
            if (storeExtractedInVersionKey && string.IsNullOrWhiteSpace(version))
            {
                return;
            }

            file.AddProperty(versionKey, storeExtractedInVersionKey ? version : versionExpression);

            if (hasInconsistentVersions)
            {
                file.AddProperty(inconsistentVersionKey, bool.TrueString.ToLowerInvariant());
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                return;
            }

            file.AddProperty(majorVersionKey, version);
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
                                 .Where(v => v.Version.StartsWith("python", StringComparison.OrdinalIgnoreCase))
                                 .Select(v => new
                                 {
                                     Version = v,
                                     ParsedVersion = ParseVersion(v.Version)
                                 })
                                 .OrderBy(v => v.ParsedVersion is null ? 1 : 0)
                                 .ThenBy(v => v.ParsedVersion);

            foreach (var version in versions)
            {
                _pythonVersions.Add(
                    version.Version.Version,
                    new PythonVersion
                    {
                        Version = version.Version.Version,
                        EolDate = version.Version.EolDate
                    });
            }
        }

        private static bool MatchesExpression(Version candidateVersion, string versionExpression)
        {
            var matches = Regex.Matches(versionExpression, @"(?<operator><=|>=|<|>|==|!=|~=|\^)?\s*(?<version>\d+(?:\.\d+){0,2})");
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
                "!=" => comparison != 0,
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

        private static string GetNormalizedComparisonVersion(string versionExpression, Func<string, string> extractVersion)
        {
            var extractedVersion = extractVersion(versionExpression);
            return string.IsNullOrWhiteSpace(extractedVersion)
                ? versionExpression.Trim()
                : extractedVersion.Trim();
        }

        private static string ExtractLowestSpecifiedVersion(string versionExpression)
        {
            var matches = Regex.Matches(versionExpression, @"\d+(?:\.\d+){0,2}");
            if (matches.Count == 0)
            {
                return string.Empty;
            }

            var selectedVersion = matches
                .Select(match => ExtractNormalizedVersion(match.Value))
                .Select(version => new
                {
                    Version = version,
                    ComparableVersion = TryParseComparableVersion(version, out var comparableVersion) ? comparableVersion : null
                })
                .Where(item => item.ComparableVersion != null)
                .OrderBy(item => item.ComparableVersion)
                .FirstOrDefault();

            return selectedVersion?.Version ?? ExtractNormalizedVersion(matches[0].Value);
        }

        private static string ExtractNormalizedVersionFromRuntimeVersion(string runtimeVersion)
        {
            const string pythonPrefix = "python ";

            var trimmedVersion = runtimeVersion.StartsWith(pythonPrefix, StringComparison.OrdinalIgnoreCase)
                ? runtimeVersion[pythonPrefix.Length..].Trim()
                : runtimeVersion.Trim();

            return ExtractNormalizedVersion(trimmedVersion);
        }

        private static bool TryParseComparableVersion(string version, out Version comparableVersion)
        {
            comparableVersion = default!;

            if (string.IsNullOrWhiteSpace(version))
            {
                return false;
            }

            var match = Regex.Match(version, @"^\d+(?:\.\d+){0,2}$");
            if (!match.Success)
            {
                return false;
            }

            comparableVersion = Version.Parse(match.Value);
            return true;
        }

        #endregion
    }
}
