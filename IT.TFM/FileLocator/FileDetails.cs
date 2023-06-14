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
    public static class FileDetails
    {
        public static async Task GetDetailsAsync(int totalThreads, bool forceDetails)
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
                var errorMsg = string.Empty;
                bool hasError = false;

                if (!repoItem.OrgName.Equals(currentScanner) || scanner == null)
                {
                    currentScanner = repoItem.OrgName;
                    scanner = ScannerFactory.GetScanner(currentScanner);
                }

                if (repoItem.RepositoryTotalFiles == 0)
                {
                    continue;
                }

                try
                {
                    await scanner.LoadFiles(repoItem.ProjectId, repoItem.RepositoryId);
                }
                catch (OutOfMemoryException)
                {
                    errorMsg = "OutOfMemoryException - unable to download repository.";
                    hasError = true;
                }

                if (hasError)
                {
                    // TODO: update repo.TooBig = true
                    continue;
                }

                var fileItems = reader.Read(repoItem.RepositoryId.ToString());
                var fileList = await scanner.Files(repoItem.ProjectId, 
                                                   new ProjectData.Repository { Id = repoItem.RepositoryId,
                                                                                DefaultBranch = repoItem.RepositoryDefaultBranch });

                Parallel.ForEach(fileItems, options, (fileItem) =>
                {
                    var azDoFiles = fileList.Where(f => f.Id == fileItem.Id);
                    var azDoFile = azDoFiles.SingleOrDefault(f => f.Path == fileItem.Path);
                    if (azDoFile == null || azDoFile.CommitId == fileItem.CommitId)
                    {
                        // TODO - flag file as deleted
                        return;
                    }

                    var fileInfo = new ProjectData.FileItem
                    {
                        Id = fileItem.Id,
                        FileType = fileItem.FileType,
                        Path = fileItem.Path,
                        Url = fileItem.Url,
                        CommitId = azDoFile.CommitId
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
                            CommitId = fileData.CommitId,
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
