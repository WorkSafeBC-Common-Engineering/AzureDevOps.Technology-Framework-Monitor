﻿using System;
using System.Collections.Generic;

namespace ProjectData
{
    public class FileItem
    {
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
                Console.WriteLine(ex.ToString());
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
