using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public static class FileDetails
    {
        public static async Task GetDetailsAsync(int totalThreads, bool forceDetails, string projectId, string repositoryId)
        {
            Settings.Initialize();

            IReadFileItem reader = StorageFactory.GetFileItemReader();

            IScanner scanner = null;
            var currentScanner = string.Empty;
            var currentBasePath = string.Empty;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            IReadRepoList repoReader = StorageFactory.GetRepoListReader();
            foreach (var repoItem in repoReader.Read(projectId, repositoryId))
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
                    currentBasePath = scanner.BasePath;
                }

                if (repoItem.RepositoryTotalFiles == 0)
                {
                    continue;
                }

                try
                {                    
                    await scanner.LoadFiles(repoItem.ProjectId, repoItem.RepositoryId, repoItem.RepositoryDefaultBranch);
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

                var configYamlFiles = new ConcurrentBag<ProjectData.FileItem>();

                var deleteList = new ConcurrentBag<DataModels.FileDetails>();

                Parallel.ForEach(fileItems, options, (fileItem) =>
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($"*** Thread Start: {Environment.CurrentManagedThreadId} >>> FileDetails - GetDetailsAsync(): Processing {fileItem.Path}");
                    }

                    var azDoFiles = fileList.Where(f => f.Path.Equals(fileItem.Path, StringComparison.InvariantCultureIgnoreCase));
                    if (Parameters.Settings.ExtendedLogging && azDoFiles.Count() > 1)
                    {
                        Console.WriteLine($" >>> FileDetails - GetDetailsAsync(): !!! Multiple files returned for {fileItem.Path} - Url = {fileItem.Url}");
                    }

                    var azDoFile = azDoFiles.FirstOrDefault();
                    if (Parameters.Settings.ExtendedLogging && azDoFiles.Count() == 1)
                    {
                        Console.WriteLine($" >>> FileDetails - GetDetailsAsync(): Matched AzDo file {fileItem.Path}");
                    }

                    if (azDoFile == null)
                    {
                        if (Parameters.Settings.ExtendedLogging)
                        {
                            Console.WriteLine($"File Details: deleting file {fileItem.Path} in Project {repoItem.ProjectName}, Repository {repoItem.RepositoryName}");
                        }

                        DataModels.FileDetails fileDetails = new()
                        {
                            Id = fileItem.Id,
                            Url = fileItem.Url,
                            FileType = fileItem.FileType,
                            Path = fileItem.Path,
                            CommitId = fileItem.CommitId,
                            Repository = new RepositoryItem { RepositoryId = repoItem.RepositoryId }
                        };

                        deleteList.Add(fileDetails);

                        if (Parameters.Settings.ExtendedLogging)
                        {
                            Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId} >>> FileDetails - GetDetailsAsync(): Processing {fileItem.Path} (delete)");
                        }

                        return;
                    }
                    if (azDoFile.CommitId == fileItem.CommitId && !forceDetails)
                    {
                        if (Parameters.Settings.ExtendedLogging)
                        {
                            Console.WriteLine($"File Details: no change to file {azDoFile.Path} in Project {repoItem.ProjectName}, Repository {repoItem.RepositoryName}");
                            Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId} >>> FileDetails - GetDetailsAsync(): Processing {fileItem.Path}");
                        }

                        return;
                    }

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId} >>> FileDetails - GetDetailsAsync(): Processing {fileItem.Path} (start of update)");
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
                        if (Parameters.Settings.ExtendedLogging)
                        {
                            Console.WriteLine($"File Details: updating details for file {azDoFile.Path} in Project {repoItem.ProjectName}, Repository {repoItem.RepositoryName}");
                        }

                        if (fileData.FileType == FileItemType.YamlPipeline && fileData.PipelineProperties.Count > 0)
                        {
                            fileData.RepositoryId = repoItem.RepositoryId;

                            // If this is a config.yml file, we need to update this later to ensure that the pipeline itself has been created or updated.
                            if (fileData.PipelineProperties.ContainsKey("IsConfigFile"))
                            {
                                configYamlFiles.Add(fileData);
                            }

                            else
                            {
                                var pipelineWriter = StorageFactory.GetPipelineWriter();
                                pipelineWriter.AddProperties(fileData);
                            }
                        }

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

                        IWriteFileDetails writer = StorageFactory.GetFileDetailsWriter();
                        writer.Write(fileDetails, forceDetails);
                    }

                    if (fileInfo.FileType != FileItemType.NoMatch)
                    {
                        fileInfo.RepositoryId = repoItem.RepositoryId;
                        var metrics = GetMetrics(fileInfo, currentBasePath);
                        if (metrics != null)
                        {
                            var writer = Storage.StorageFactory.GetStorageWriter();
                            writer.SaveMetrics(fileInfo, metrics);
                        }
                    }

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId} >>> FileDetails - GetDetailsAsync(): Processing {fileItem.Path}");
                    }
                });

                if (!configYamlFiles.IsEmpty)
                {
                    var pipelineWriter = StorageFactory.GetPipelineWriter();
                    var pipelineReader = StorageFactory.GetPipelineReader();

                    foreach (var fileDetails in configYamlFiles)
                    {
                        var portfolio = fileDetails.PipelineProperties["portfolio"];
                        var product = fileDetails.PipelineProperties["product"];

                        var pipelines = pipelineReader.FindPipelines(repoItem.ProjectId, fileDetails.RepositoryId, portfolio, product);
                        if (!pipelines.Any())
                        {
                            continue;
                        }

                        var environments = fileDetails.PipelineProperties["Environments"].Split('|');
                        foreach (var pipeline in pipelines)
                        {
                            pipeline.Environments = environments;
                            pipelineWriter.Write(pipeline);
                        }
                    }
                }

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

        private static Metrics GetMetrics(ProjectData.FileItem file, string basePath)
        {
            var scanner = MetricsScannerFactory.GetScanner(file.FileType.ToString());
            var metrics = scanner.Get(file, basePath);
            return metrics;
        }
    }
}
