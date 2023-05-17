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

            IReadRepoList repoReader = StorageFactory.GetRepoListReader();
            foreach (var repoItem in repoReader.Read())
            {
                if (!repoItem.OrgName.Equals(currentScanner) || scanner == null)
                {
                    currentScanner = repoItem.OrgName;
                    scanner = ScannerFactory.GetScanner(currentScanner);
                }

                var task = scanner.LoadFiles(repoItem.ProjectId, repoItem.RepositoryId);
                task.Wait();

                var fileItems = reader.Read(repoItem.RepositoryId.ToString());

                Parallel.ForEach(fileItems, options, (fileItem) =>
                {
                    var fileInfo = new ProjectData.FileItem
                    {
                        Id = fileItem.Id,
                        FileType = fileItem.FileType,
                        Path = fileItem.Path,
                        Url = fileItem.Url,
                        SHA1 = fileItem.SHA1
                    };

                    var fileData = scanner.FileDetails(repoItem.ProjectId, repoItem.RepositoryId, fileInfo);
                    if (fileData != null)
                    {
                        var fileDetails = new DataModels.FileDetails
                        {
                            Repository = fileItem.Repository ?? new RepositoryItem { RepositoryId = repoItem.RepositoryId },
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

                scanner.DeleteFiles();
            }
        }
    }
}
