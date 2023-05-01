using ProjectData.Interfaces;
using ProjectScanner;
using RepoScan.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class FileDetails
    {
        public void GetDetails(int totalThreads, bool forceDetails)
        {
            Settings.Initialize();

            IReadFileItem reader = StorageFactory.GetFileItemReader();
            IWriteFileDetails writer = StorageFactory.GetFileDetailsWriter();

            IScanner scanner = null;
            var currentScanner = string.Empty;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            Parallel.ForEach(reader.ReadDetails(), options, (fileItem) =>
            {
                if (!fileItem.Repository.OrgName.Equals(currentScanner) || scanner == null)
                {
                    currentScanner = fileItem.Repository.OrgName;
                    scanner = ScannerFactory.GetScanner(currentScanner);
                }

                var fileInfo = new ProjectData.FileItem
                {
                    Id = fileItem.Id,
                    FileType = fileItem.FileType,
                    Path = fileItem.Path,
                    Url = fileItem.Url,
                    SHA1 = fileItem.SHA1
                };

                var fileData = scanner.FileDetails(fileItem.Repository.RepositoryId, fileInfo);
                if (fileData != null)
                {
                    var fileDetails = new DataModels.FileDetails
                    {
                        Repository = fileItem.Repository,
                        Id = fileItem.Id,
                        FileType = fileItem.FileType,
                        Path = fileItem.Path,
                        Url = fileItem.Url,
                        SHA1 = fileData.SHA1,
                        References = fileData.References,
                        UrlReferences = fileData.UrlReferences,
                        PackageReferences = fileData.PackageReferences,
                        Properties = new SerializableDictionary<string, string>(fileData.Properties),
                        FilteredItems = new SerializableDictionary<string, string>(fileData.FilteredItems)
                    };

                    writer.Write(fileDetails, forceDetails);
                }
            });
        }
    }
}
