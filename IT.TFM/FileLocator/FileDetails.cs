using Microsoft.Extensions.Logging;

using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class FileDetails
    {
        private readonly ILogger<FileDetails> Logger;
        private readonly ILogger<ProjectData.FileItem> fileLogger;

        public FileDetails(ILogger<FileDetails> logger, ILogger<ProjectData.FileItem> fileLogger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(fileLogger);
            Logger = logger;
            this.fileLogger = fileLogger;
        }

        public async Task GetDetailsAsync(int totalThreads, bool forceDetails, string projectId, string repositoryId)
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

                var pipelineFiles = new ConcurrentBag<ProjectData.FileItem>();
                var configYamlFiles = new ConcurrentBag<ProjectData.FileItem>();

                var deleteList = new ConcurrentBag<DataModels.FileDetails>();

                Parallel.ForEach(fileItems, options, async (fileItem) =>
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("Thread Start: {ThreadId} >>> FileDetails - GetDetailsAsync(): Processing {FilePath}", Environment.CurrentManagedThreadId, fileItem.Path);
                    }

                    var azDoFiles = fileList.Where(f => f.Path.Equals(fileItem.Path, StringComparison.InvariantCultureIgnoreCase));
                    if (Parameters.Settings.ExtendedLogging && azDoFiles.Count() > 1)
                    {
                        Logger.LogWarning("FileDetails - GetDetailsAsync(): Multiple files returned for {FilePath} - Url = {Url}", fileItem.Path, fileItem.Url);
                    }

                    var azDoFile = azDoFiles.FirstOrDefault();
                    if (Parameters.Settings.ExtendedLogging && azDoFiles.Count() == 1)
                    {
                        Logger.LogInformation("FileDetails - GetDetailsAsync(): Matched AzDo file {FilePath}", fileItem.Path);
                    }

                    if (azDoFile == null)
                    {
                        if (Parameters.Settings.ExtendedLogging)
                        {
                            Logger.LogInformation("File Details: deleting file {FilePath} in Project {ProjectName}, Repository {RepositoryName}", fileItem.Path, repoItem.ProjectName, repoItem.RepositoryName);
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
                            Logger.LogInformation("Thread End: {ThreadId} >>> FileDetails - GetDetailsAsync(): Processing {FilePath} (delete)", Environment.CurrentManagedThreadId, fileItem.Path);
                        }

                        return;
                    }
                    if (azDoFile.CommitId == fileItem.CommitId && !forceDetails)
                    {
                        if (Parameters.Settings.ExtendedLogging)
                        {
                            Logger.LogInformation("File Details: no change to file {FilePath} in Project {ProjectName}, Repository {RepositoryName}", azDoFile.Path, repoItem.ProjectName, repoItem.RepositoryName);
                            Logger.LogInformation("Thread End: {ThreadId} >>> FileDetails - GetDetailsAsync(): Processing {FilePath}", Environment.CurrentManagedThreadId, fileItem.Path);
                        }

                        return;
                    }

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("Thread End: {ThreadId} >>> FileDetails - GetDetailsAsync(): Processing {FilePath} (start of update)", Environment.CurrentManagedThreadId, fileItem.Path);
                    }

                    var fileInfo = new ProjectData.FileItem(fileLogger)
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
                            Logger.LogInformation("File Details: updating details for file {FilePath} in Project {ProjectName}, Repository {RepositoryName}", azDoFile.Path, repoItem.ProjectName, repoItem.RepositoryName);
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
                                pipelineFiles.Add(fileData);
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
                            Properties = [with(fileData.Properties)],
                            FilteredItems = [with(fileData.FilteredItems)]
                        };

                        IWriteFileDetails writer = StorageFactory.GetFileDetailsWriter();
                        writer.Write(fileDetails, forceDetails);
                    }

                    if (fileInfo.FileType != FileItemType.NoMatch)
                    {
                        fileInfo.RepositoryId = repoItem.RepositoryId;
                        var metrics = GetMetrics(fileInfo, currentBasePath);

                        var writer = Storage.StorageFactory.GetStorageWriter();

                        foreach (var projectFile in metrics.Keys)
                        {
                            var projectItem = fileItems.FirstOrDefault(f => f.Path.EndsWith(projectFile, StringComparison.OrdinalIgnoreCase));
                            if (projectItem != null)
                            {
                                var metricsValues = metrics[projectFile];
                                await writer.SaveMetricsAsync(fileInfo, metricsValues, null);
                            }
                        }
                    }

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("Thread End: {ThreadId} >>> FileDetails - GetDetailsAsync(): Processing {FilePath}", Environment.CurrentManagedThreadId, fileItem.Path);
                    }
                });

                if (!configYamlFiles.IsEmpty)
                {
                    var pipelineWriter = StorageFactory.GetPipelineWriter();
                    var pipelineReader = StorageFactory.GetPipelineReader();

                    foreach (var pipelineFile in pipelineFiles)
                    {
                        Logger.LogInformation("Linking config.yml to pipeline file {FilePath} in Project {ProjectName}, Repository {RepositoryName}", pipelineFile.Path, repoItem.ProjectName, repoItem.RepositoryName);

                        var portfolio = pipelineFile.PipelineProperties.TryGetValue("portfolio", out string value) ? value : string.Empty;
                        var product = pipelineFile.PipelineProperties.TryGetValue("product", out value) ? value : string.Empty;

                        if (string.IsNullOrEmpty(portfolio) || string.IsNullOrEmpty(product))
                        {
                            Logger.LogWarning("Skipping pipeline {FilePath} as it does not have portfolio or product defined.", pipelineFile.Path);
                            continue;
                        }

                        var configPath = $"/deploy/{portfolio}-{product}-config.yml";
                        var configFile = configYamlFiles.FirstOrDefault(f => f.Path.Equals(configPath, StringComparison.InvariantCultureIgnoreCase));
                        
                        if (configFile == null)
                        {
                            configPath = $"/deploy/{portfolio}.{product}-config.yml";
                            configFile = configYamlFiles.FirstOrDefault(f => f.Path.Equals(configPath, StringComparison.InvariantCultureIgnoreCase));
                        }

                        if (configFile == null)
                        {
                            Logger.LogWarning("No config.yml file found for portfolio {Portfolio} and product {Product} in Project {ProjectName}, Repository {RepositoryName}", portfolio, product, repoItem.ProjectName, repoItem.RepositoryName);
                            continue;
                        }

                        var pipelines = pipelineReader.FindPipelines(pipelineFile);
                        if (!pipelines.Any())
                        {
                            continue;
                        }

                        var environments = configFile.PipelineProperties["Environments"].Split('|');
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

        private static Dictionary<string, Metrics> GetMetrics(ProjectData.FileItem file, string basePath)
        {
            var scanner = MetricsScannerFactory.GetScanner(file.FileType.ToString());
            var metrics = scanner.Get(file, basePath);
            return metrics;
        }
    }
}
