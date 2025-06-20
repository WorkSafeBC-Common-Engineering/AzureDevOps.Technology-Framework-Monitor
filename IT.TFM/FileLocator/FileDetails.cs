﻿using ProjectData;
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
    public static class FileDetails
    {
        public static async Task GetDetailsAsync(int totalThreads, bool forceDetails, string projectId, string repositoryId, string[] excludedProjects)
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
                        Console.WriteLine($"*** Thread End: {Environment.CurrentManagedThreadId} >>> FileDetails - GetDetailsAsync(): Processing {fileItem.Path}");
                    }
                });

                if (!configYamlFiles.IsEmpty)
                {
                    var pipelineWriter = StorageFactory.GetPipelineWriter();
                    var pipelineReader = StorageFactory.GetPipelineReader();

                    foreach (var pipelineFile in pipelineFiles)
                    {
                        Console.WriteLine($"Linking config.yml to pipeline file {pipelineFile.Path} in Project {repoItem.ProjectName}, Repository {repoItem.RepositoryName}");

                        var portfolio = pipelineFile.PipelineProperties.TryGetValue("portfolio", out string value) ? value : string.Empty;
                        var product = pipelineFile.PipelineProperties.TryGetValue("product", out value) ? value : string.Empty;

                        if (string.IsNullOrEmpty(portfolio) || string.IsNullOrEmpty(product))
                        {
                            Console.WriteLine($"Skipping pipeline {pipelineFile.Path} as it does not have portfolio or product defined.");
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
                            Console.WriteLine($"No config.yml file found for portfolio {portfolio} and product {product} in Project {repoItem.ProjectName}, Repository {repoItem.RepositoryName}");
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
