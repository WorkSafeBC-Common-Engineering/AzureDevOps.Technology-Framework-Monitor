using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectData
{
    public enum MatchType
    {
        EndsWith,
        Exact
    }
    
    public class FileMatch
    {
        public MatchType MatchType { get; private set; }

        public string MatchOn { get; private set; }

        public string[] Exceptions { get; private set; }

        internal static FileMatch MatchOnFile(FileItemType fileType)
        {
            return fileType switch
            {
                FileItemType.VSSolution => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".sln",
                    Exceptions = []
                },
                FileItemType.VB6Project => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".vbp",
                    Exceptions = []
                },
                FileItemType.CSProject => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".csproj",
                    Exceptions = []
                },
                FileItemType.VBProject => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".vbproj",
                    Exceptions = []
                },
                FileItemType.SqlProject => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".sqlproj",
                    Exceptions = []
                },
                FileItemType.VSConfig => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".config",
                    Exceptions = ["packages.config"]
                },
                FileItemType.NuGetPkgConfig => new FileMatch
                {
                    MatchType = MatchType.Exact,
                    MatchOn = "packages.config",
                    Exceptions = []
                },
                FileItemType.Nuspec => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".nuspec",
                    Exceptions = []
                },
                FileItemType.NpmPackage => new FileMatch
                {
                    MatchType = MatchType.Exact,
                    MatchOn = "package.json",
                    Exceptions = []
                },
                FileItemType.YamlPipeline => new FileMatch
                {
                    MatchType = MatchType.EndsWith,
                    MatchOn = ".yml",
                    Exceptions = []
                },
                _ => throw new ArgumentException("Invalid parameter - value does not exist", nameof(fileType)),
            };
        }
    }

    public static class StringExtension
    {
        private static readonly Dictionary<FileItemType, FileMatch> fileMatches = new()
        {
            {FileItemType.VSSolution, FileMatch.MatchOnFile(FileItemType.VSSolution) },
            {FileItemType.VB6Project, FileMatch.MatchOnFile(FileItemType.VB6Project) },
            {FileItemType.CSProject, FileMatch.MatchOnFile(FileItemType.CSProject) },
            {FileItemType.VBProject, FileMatch.MatchOnFile(FileItemType.VBProject) },
            {FileItemType.SqlProject, FileMatch.MatchOnFile(FileItemType.SqlProject) },
            {FileItemType.VSConfig, FileMatch.MatchOnFile(FileItemType.VSConfig) },
            {FileItemType.NuGetPkgConfig, FileMatch.MatchOnFile(FileItemType.NuGetPkgConfig) },
            {FileItemType.Nuspec, FileMatch.MatchOnFile(FileItemType.Nuspec) },
            {FileItemType.NpmPackage, FileMatch.MatchOnFile(FileItemType.NpmPackage) },
            {FileItemType.YamlPipeline, FileMatch.MatchOnFile(FileItemType.YamlPipeline) }
        };

        public static FileItemType GetMatchedFileType(this string filename)
        {
            foreach (var match in fileMatches)
            {
                switch (match.Value.MatchType)
                {
                    case MatchType.EndsWith:
                        if (filename.EndsWith(match.Value.MatchOn, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (NotAnException(match.Value.Exceptions, filename))
                            {
                                return match.Key;
                            }
                        }
                        break;

                    case MatchType.Exact:
                        var file = Path.GetFileName(filename);
                        if (match.Value.MatchOn.Equals(file, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return match.Key;
                        }
                        break;
                }
            }

            return FileItemType.NoMatch;
        }

        public static bool Matches(this string filename, FileItemType fileType)
        {
            var match = FileMatch.MatchOnFile(fileType);

            switch (match.MatchType)
            {
                case MatchType.EndsWith:
                    if (filename.EndsWith(match.MatchOn, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return NotAnException(match.Exceptions, filename);
                    }
                    break;

                case MatchType.Exact:
                    var file = Path.GetFileName(filename);
                    return match.MatchOn.Equals(file, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private static bool NotAnException(string[] exceptions, string filename)
        {
            var file = Path.GetFileName(filename);

            return !exceptions.Any(e => e.Equals(file, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
