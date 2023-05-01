using ProjectData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileFiltering
{
    public class Filter
    {
        #region Private Members

        private const string filterType = "FileExtension";
        private const string filterContent = "FileContent";

        private static string[] fileExtensions;
        private static string[] matches;

        #endregion

        #region Constructors

        static Filter()
        {
            var filterData = FilterConfiguration.Configuration;
            Initialize(filterData);
        }

        #endregion

        #region Public Methods and Properties
        
        public static bool CanFilterFile(FileItem file)
        {
            return HasMatchingExtension(file);
        }

        public static void FilterFile(FileItem file, string[] content)
        {
            if (!HasMatchingExtension(file))
            {
                return;
            }

            FindMatchingContent(file, content);
        }

        #endregion

        #region Private Methods

        public static void Initialize(IEnumerable<FilterData> filterData)
        {
            fileExtensions = filterData.Where(f => f.FilterType.Equals(filterType, StringComparison.InvariantCultureIgnoreCase))
                                       .Select(f => f.Data.ToLower())
                                       .ToArray();

            matches = filterData.Where(f => f.FilterType.Equals(filterContent, StringComparison.InvariantCultureIgnoreCase))
                                .Select(f => f.Data.ToLower())
                                .ToArray();
        }

        private static bool HasMatchingExtension(FileItem file)
        {
            var fileExtension = Path.GetExtension(file.Path.ToLower());
            return fileExtensions.Contains(fileExtension);
        }

        private static void FindMatchingContent(FileItem file, string[] content)
        {
            foreach (var match in matches)
            {
                foreach (var line in content)
                {
                    if (line.ToLower().Contains(match))
                    {
                        file.AddReference($"FileFilter: {match}");
                        return;
                    }
                }
            }
        }

        #endregion
    }
}
