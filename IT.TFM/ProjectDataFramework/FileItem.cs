using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace ProjectData
{
    public class FileItem
    {
        private readonly ILogger<FileItem> Logger;

        public FileItem(ILogger<FileItem> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            Logger = logger;
        }

        #region Public Properties

        public FileItemType FileType { get; set; }

        public string Id { get; set; }

        public string Path { get; set; }

        public string Url { get; set; }

        public string CommitId { get; set; }

        public int StorageId { get; set; }

        public Dictionary<string, string> Properties { get; } = [];

        public List<string> References { get; } = [];

        public List<UrlReference> UrlReferences { get; } = [];

        public List<PackageReference> PackageReferences { get; } = [];

        public Dictionary<string, string> FilteredItems { get; } = [];

        public Guid RepositoryId { get; set; } = Guid.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public Dictionary<string, string> PipelineProperties { get; } = [];

        #endregion

        #region Public Methods

        public void AddProperty(string name, string value)
        {
            try
            {
                Properties.Add(name, value);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Unable to add property {PropertyName} to file {FilePath}", name, Path);
            }
        }

        public void AddReference(string value)
        {
            References.Add(value);
        }

        public void AddUrlReference(string url, string path)
        {
            UrlReferences.Add(new UrlReference { Path = path, Url = url });
        }

        public void AddPackageReference(string packageType, string id, string version, string framework, string versionComparator)
        {
            PackageReferences.Add(new PackageReference
            {
                PackageType =  packageType ?? "UNKNOWN",
                Id = id ?? string.Empty,
                Version = version ?? string.Empty,
                VersionComparator = versionComparator ?? string.Empty,
                FrameworkVersion = framework ?? string.Empty
            });
        }

        public void AddPackageProperty(string name, string value)
        {
            PipelineProperties.Add(name, value);
        }

        #endregion
    }
}
