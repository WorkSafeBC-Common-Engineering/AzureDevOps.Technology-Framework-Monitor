using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PythonFileParser
{
    public partial class PythonDockerfileParser : IFileParser
    {
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

            ParseVersionFile(
                file,
                PythonCommon.SelectHighestVersionExpression(versionExpressions, PythonCommon.ExtractNormalizedVersion),
                PythonCommon.HasInconsistentVersions(versionExpressions, PythonCommon.ExtractNormalizedVersion));
        }

        #endregion

        #region Private Methods

        private static bool TryAddArgValue(string line, Dictionary<string, string> args)
        {
            var argMatch = MyRegex.Match(line);
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

            var fromMatch = MyRegex1.Match(trimmedLine);
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
            return MyRegex2.IsMatch(imageName);
        }

        private static string ResolveTagVariables(string tag, IReadOnlyDictionary<string, string> args)
        {
            var resolvedTag = tag;

            resolvedTag = MyRegex3.Replace(resolvedTag, match =>
            {
                var name = match.Groups["name"].Value;
                return args.TryGetValue(name, out var value) ? value : match.Value;
            });

            resolvedTag = MyRegex4.Replace(resolvedTag, match =>
            {
                var name = match.Groups["name"].Value;
                return args.TryGetValue(name, out var value) ? value : match.Value;
            });

            return resolvedTag.Trim();
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
                PythonCommon.ExtractNormalizedVersion,
                hasInconsistentVersions,
                storeExtractedInVersionKey: true);
        }

        [GeneratedRegex(@"^\s*arg\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<value>.+?)\s*$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex MyRegex { get; }

        [GeneratedRegex(@"^\s*from(?:\s+--[^\s]+)*\s+(?<image>[^\s]+)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex MyRegex1 { get; }

        [GeneratedRegex(@"(^|[\/\-_\.])python([\/\-_\.]|$)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex MyRegex2 { get; }

        [GeneratedRegex(@"\$\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\}")]
        private static partial Regex MyRegex3 { get; }

        [GeneratedRegex(@"\$(?<name>[A-Za-z_][A-Za-z0-9_]*)")]
        private static partial Regex MyRegex4 { get; }

        #endregion
    }
}