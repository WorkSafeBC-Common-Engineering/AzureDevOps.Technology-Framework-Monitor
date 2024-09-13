using ProjectData;

using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public class FileDetails
    {
        #region Public Properties

        public RepositoryItem Repository { get; set; }

        public FileItemType FileType { get; set; }

        public string Id { get; set; }

        public string Path { get; set; }

        public string Url { get; set; }

        public string CommitId { get; set; }

        public List<string> References { get; set; }

        public List<UrlReference> UrlReferences { get; set; }

        public List<PackageReference> PackageReferences { get; set; }

        public List<PackageReferenceIssue> PackageReferenceIssues { get; set; }

        public SerializableDictionary<string, string> Properties { get; set; } = [];

        public SerializableDictionary<string, string> FilteredItems { get; set; } = [];

        #endregion
    }
}
