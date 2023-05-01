using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            switch (fileType)
            {
                case FileItemType.VSSolution:
                    return new FileMatch
                    {
                        MatchType = MatchType.EndsWith,
                        MatchOn = ".sln",
                        Exceptions = new string[] { }
            };

                case FileItemType.VB6Project:
                    return new FileMatch
                    {
                        MatchType = MatchType.EndsWith,
                        MatchOn = ".vbp",
                        Exceptions = new string[] { }
                    };

                case FileItemType.CSProject:
                    return new FileMatch
                    {
                        MatchType = MatchType.EndsWith,
                        MatchOn = ".csproj",
                        Exceptions = new string[] { }
                    };

                case FileItemType.VBProject:
                    return new FileMatch
                    {
                        MatchType = MatchType.EndsWith,
                        MatchOn = ".vbproj",
                        Exceptions = new string[] { }
                    };

                case FileItemType.SqlProject:
                    return new FileMatch
                    {
                        MatchType = MatchType.EndsWith,
                        MatchOn = ".sqlproj",
                        Exceptions = new string[] { }
                    };

                case FileItemType.VSConfig:
                    return new FileMatch
                    {
                        MatchType = MatchType.EndsWith,
                        MatchOn = ".config",
                        Exceptions = new string[] { "packages.config" }
                    };

                case FileItemType.NuGetPkgConfig:
                    return new FileMatch
                    {
                        MatchType = MatchType.Exact,
                        MatchOn = "packages.config",
                        Exceptions = new string[] { }
                    };

                case FileItemType.NpmPackage:
                    return new FileMatch
                    {
                        MatchType = MatchType.Exact,
                        MatchOn = "package.json",
                        Exceptions = new string[] { }
                    };

                default:
                    throw new ArgumentException("Invalid parameter - value does not exist", "itemType");
            }
        }
    }

    public static class StringExtension
    {
        private static readonly Dictionary<FileItemType, FileMatch> fileMatches = new Dictionary<FileItemType, FileMatch>
        {
            {FileItemType.VSSolution, FileMatch.MatchOnFile(FileItemType.VSSolution) },
            {FileItemType.VB6Project, FileMatch.MatchOnFile(FileItemType.VB6Project) },
            {FileItemType.CSProject, FileMatch.MatchOnFile(FileItemType.CSProject) },
            {FileItemType.VBProject, FileMatch.MatchOnFile(FileItemType.VBProject) },
            {FileItemType.SqlProject, FileMatch.MatchOnFile(FileItemType.SqlProject) },
            {FileItemType.VSConfig, FileMatch.MatchOnFile(FileItemType.VSConfig) },
            {FileItemType.NuGetPkgConfig, FileMatch.MatchOnFile(FileItemType.NuGetPkgConfig) },
            {FileItemType.NpmPackage, FileMatch.MatchOnFile(FileItemType.NpmPackage) }
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
