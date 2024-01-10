using ProjectData.Interfaces;
using ProjectScanner;
using RepoScan.DataModels;
using System;
using System.Collections.Concurrent;
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

            IScanner scanner = null;
            var currentScanner = string.Empty;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            IReadRepoList repoReader = StorageFactory.GetRepoListReader();
            foreach (var repoItem in repoReader.Read())
            {
                // Skip any repos that have been flagged as deleted or No Scan
                if (repoItem.ProjectNoScan || repoItem.ProjectIsDeleted || repoItem.RepositoryNoScan || repoItem.IsDeleted)
                {
                    continue;
                }

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

                var deleteList = new ConcurrentBag<DataModels.FileDetails>();
                Parallel.ForEach(fileItems, options, (fileItem) =>
                {
#if DEBUG
                    Console.WriteLine($"*** Thread Start: {Environment.CurrentManagedThreadId}");
#endif

                    //TODO: Create a pool of writer items (one per totalThread) to create the necessary DB connections ahead of time.
                    IWriteFileDetails writer = StorageFactory.GetFileDetailsWriter();

                    var azDoFiles = fileList.Where(f => f.Id == fileItem.Id);
                    var azDoFile = azDoFiles.SingleOrDefault(f => f.Path == fileItem.Path);

                    if (azDoFile == null)
                    {
                        DataModels.FileDetails fileDetails = new()
                        {
                            Id = fileItem.Id,
                            Url = fileItem.Url,
                            FileType = fileItem.FileType,
                            Path = fileItem.Path,
                            CommitId = fileItem.CommitId, 
                            Repository = new RepositoryItem { RepositoryId = repoItem.RepositoryId }
                        }; 

                        deleteList.Add(fileDetails );
#if DEBUG
                        Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId}");
#endif
                        return;
                    }
                    if (azDoFile.CommitId == fileItem.CommitId && !forceDetails)
                    {
#if DEBUG
                        Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId}");
#endif
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
#if DEBUG
                    Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId}");
#endif
                });

                if (!deleteList.IsEmpty)
                {
                    IWriteFileDetails writer = StorageFactory.GetFileDetailsWriter();
                    foreach (var fileDetails in deleteList)
                    {
                        writer.Delete(fileDetails);
                    }
                }

                scanner.DeleteFiles();
            }
        }
    }
}
