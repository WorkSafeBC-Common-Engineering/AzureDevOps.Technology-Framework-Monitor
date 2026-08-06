using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    public class PythonDockerfileParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersionDockerfile";
        private const string majorVersionKey = "PythonMajorVersionDockerfile";
        private const string inconsistentVersionKey = "PythonInconsistentVersion";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var args = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var versionExpressions = new List<string>();

            for (int i = 0; i < content.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(content[i]))
                {
                    continue;
                }

                TryAddArgValue(content[i], args);

                if (TryGetVersionExpression(content[i], args, out var versionExpression))
                {
                    versionExpressions.Add(versionExpression);
                }
            }

            ParseVersionFile(file, SelectVersionExpression(versionExpressions), HasInconsistentVersions(versionExpressions));
        }

        #endregion

        #region Private Methods

        private static bool TryAddArgValue(string line, IDictionary<string, string> args)
        {
            var argMatch = Regex.Match(line, @"^\s*arg\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<value>.+?)\s*$", RegexOptions.IgnoreCase);
            if (!argMatch.Success)
            {
                return false;
            }

            var argName = argMatch.Groups["name"].Value.Trim();
            var argValue = argMatch.Groups["value"].Value.Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(argName) || string.IsNullOrWhiteSpace(argValue))
            {
                return false;
            }

            var commentIndex = argValue.IndexOf('#');
            if (commentIndex >= 0)
            {
                argValue = argValue[..commentIndex].Trim();
            }

            args[argName] = argValue;
            return true;
        }

        private static bool TryGetVersionExpression(string line, IReadOnlyDictionary<string, string> args, out string versionExpression)
        {
            versionExpression = string.Empty;

            var trimmedLine = line.Trim();
            if (!trimmedLine.StartsWith("from ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var commentIndex = trimmedLine.IndexOf('#');
            if (commentIndex >= 0)
            {
                trimmedLine = trimmedLine[..commentIndex].Trim();
            }

            var fromMatch = Regex.Match(trimmedLine, @"^\s*from(?:\s+--[^\s]+)*\s+(?<image>[^\s]+)", RegexOptions.IgnoreCase);
            if (!fromMatch.Success)
            {
                return false;
            }

            var imageReference = fromMatch.Groups["image"].Value.Trim();
            if (!TryGetImageAndTag(imageReference, out var imageName, out var imageTag))
            {
                return false;
            }

            if (!IsPythonImage(imageName))
            {
                return false;
            }

            var resolvedTag = ResolveTagVariables(imageTag, args);
            if (string.IsNullOrWhiteSpace(resolvedTag))
            {
                return false;
            }

            versionExpression = resolvedTag;
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

        private static string SelectVersionExpression(IReadOnlyList<string> versionExpressions)
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
                    ComparableVersion = TryParseComparableVersion(ExtractVersion(expression), out var comparableVersion) ? comparableVersion : null
                })
                .Where(item => item.ComparableVersion != null)
                .OrderBy(item => item.ComparableVersion)
                .FirstOrDefault();

            return selectedVersion?.Expression ?? versionExpressions[0];
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

        private static bool TryGetImageAndTag(string imageReference, out string imageName, out string imageTag)
        {
            imageName = string.Empty;
            imageTag = string.Empty;

            var referenceWithoutDigest = imageReference.Split('@')[0];
            var lastSlashIndex = referenceWithoutDigest.LastIndexOf('/');
            var lastColonIndex = referenceWithoutDigest.LastIndexOf(':');

            if (lastColonIndex <= lastSlashIndex || lastColonIndex < 0)
            {
                return false;
            }

            imageName = referenceWithoutDigest[..lastColonIndex].Trim();
            imageTag = referenceWithoutDigest[(lastColonIndex + 1)..].Trim();

            return !string.IsNullOrWhiteSpace(imageName) && !string.IsNullOrWhiteSpace(imageTag);
        }

        private static bool IsPythonImage(string imageName)
        {
            return Regex.IsMatch(imageName, @"(^|[\/\-_\.])python([\/\-_\.]|$)", RegexOptions.IgnoreCase);
        }

        private static string ResolveTagVariables(string tag, IReadOnlyDictionary<string, string> args)
        {
            var resolvedTag = tag;

            resolvedTag = Regex.Replace(resolvedTag, @"\$\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\}", match =>
            {
                var name = match.Groups["name"].Value;
                return args.TryGetValue(name, out var value) ? value : match.Value;
            });

            resolvedTag = Regex.Replace(resolvedTag, @"\$(?<name>[A-Za-z_][A-Za-z0-9_]*)", match =>
            {
                var name = match.Groups["name"].Value;
                return args.TryGetValue(name, out var value) ? value : match.Value;
            });

            return resolvedTag.Trim();
        }

        private static bool HasInconsistentVersions(IReadOnlyList<string> versionExpressions)
        {
            if (versionExpressions.Count <= 1)
            {
                return false;
            }

            var distinctVersions = versionExpressions
                .Select(GetNormalizedComparisonVersion)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            return distinctVersions > 1;
        }

        private static string GetNormalizedComparisonVersion(string versionExpression)
        {
            var extractedVersion = ExtractVersion(versionExpression);
            return string.IsNullOrWhiteSpace(extractedVersion)
                ? versionExpression.Trim()
                : extractedVersion.Trim();
        }

        private static void ParseVersionFile(FileItem file, string versionExpression, bool hasInconsistentVersions)
        {
            if (string.IsNullOrWhiteSpace(versionExpression))
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