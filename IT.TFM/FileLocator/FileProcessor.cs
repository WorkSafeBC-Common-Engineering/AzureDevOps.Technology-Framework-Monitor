using Microsoft.Extensions.Logging;

using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class FileProcessor
    {
        private readonly ILogger<FileProcessor> Logger;

        public FileProcessor(ILogger<FileProcessor> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            Logger = logger;
        }

        #region Public Methods

        public async Task GetFiles(int totalThreads, bool forceDetails, string projectId, string repositoryId, string[] excludedProjects)
        {
            Settings.Initialize();

            IReadRepoList reader = StorageFactory.GetRepoListReader();
            IWriteRepoList repoWriter = StorageFactory.GetRepoListWriter();

            var orgName = string.Empty;
            IScanner scanner = null;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            IEnumerable<Guid> projectList = new List<Guid>().AsEnumerable();

            var lastProject = Guid.Empty;
            IEnumerable<Repository> repoList = null;
            var dbProjectId = 0;

            foreach (var repoItem in reader.Read(projectId, repositoryId))
            {
                // Skip any repos that have been flagged as deleted or No Scan
                if (repoItem.ProjectNoScan || repoItem.ProjectIsDeleted || repoItem.RepositoryNoScan || repoItem.IsDeleted)
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("File Scan GetFiles(): Skipping repo {RepositoryName} - ProjectNoScan = {ProjectNoScan}, ProjectIsDeleted = {ProjectIsDeleted}, RepositoryNoScan = {RepositoryNoScan}, IsDeleted = {IsDeleted}", repoItem.RepositoryName, repoItem.ProjectNoScan, repoItem.ProjectIsDeleted, repoItem.RepositoryNoScan, repoItem.IsDeleted);
                    }

                    continue;
                }

                if (orgName != repoItem.OrgName)
                {
                    orgName = repoItem.OrgName;
                    scanner = ScannerFactory.GetScanner(orgName);

                    List<Guid> pList = [];
                    await foreach (var p in scanner.Projects(string.Empty, excludedProjects))
                    {
                        pList.Add(p.Id);
                    }
                    projectList = [.. pList];

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("File Scan, GetFiles(): Get Project IDs returned {ProjectCount} items", projectList.Count());
                    }
                }

                // Check whether the project or repo still exists - it might have been moved or deleted.
                if (!projectList.Any(p => p == repoItem.ProjectId))
                {
                    repoItem.ProjectIsDeleted = true;
                    dbProjectId = repoWriter.Write(repoItem, dbProjectId, false);

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("File Scan GetFiles(): Project {ProjectName} was deleted", repoItem.ProjectName);
                    }

                    continue;
                }

                if (lastProject != repoItem.ProjectId)
                {
                    var project = new Project
                    {
                        Id = repoItem.ProjectId,
                        Name = repoItem.ProjectName
                    };

                    repoList = await scanner.Repositories(project, string.Empty);
                    lastProject = repoItem.ProjectId;

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("File Scan GetFiles(): Project {ProjectName} returned {RepositoryCount} repositories", repoItem.ProjectName, repoList.Count());
                    }
                }

                bool repoExists = false;
                bool repoUnchanged = false;

                var r = repoList?.FirstOrDefault(r => r.Id == repoItem.RepositoryId);

                if (r != null)
                {
                    if (r.LastCommitId == repoItem.RepositoryLastCommitId)
                    {
                        Logger.LogInformation("Repo {RepositoryName} - not changed since last commit (Commit ID: {CommitId})", r.Name, r.LastCommitId);
                        repoUnchanged = true;
                    }
                    else
                    {
                        repoItem.RepositoryLastCommitId = r.LastCommitId;
                    }

                    repoExists = true;
                }

                if (!repoExists)
                {
                    // flag repo as deleted.
                    repoItem.IsDeleted = true;
                    dbProjectId = repoWriter.Write(repoItem, dbProjectId, false);

                    Logger.LogInformation("File Scan GetFiles(): repository {RepositoryName} was deleted", repoItem.RepositoryName);
                    continue;
                }

                if (repoUnchanged && !forceDetails)
                {
                    Logger.LogInformation("File Scan GetFiles(): repository {RepositoryName} is unchanged", repoItem.RepositoryName);
                    continue;
                }

                var repo = new Repository
                {
                    Id = repoItem.RepositoryId,
                    DefaultBranch = repoItem.RepositoryDefaultBranch
                };

                var fileList = await scanner.Files(repoItem.ProjectId, repo);

                Parallel.ForEach(fileList, options, (file) =>
                {
                    Logger.LogInformation("File Scan GetFiles(): Getting File: Repository = {RepositoryName}, File Path = {FilePath}", repoItem.RepositoryName, file.Path);

                    if (file.FileType != FileItemType.NoMatch || FileFiltering.Filter.CanFilterFile(file))
                    {
                        var fileItem = new DataModels.FileItem
                        {
                            Repository = repoItem,
                            FileType = file.FileType,
                            Id = file.Id,
                            Path = file.Path,
                            Url = file.Url,
                            CommitId = file.CommitId
                        };

                        // TODO: create a pool of these writer items, one per totalThreads.
                        // Then each thread could have this created ahead of time without
                        // the cost of creating a fresh DB connection every time.
                        IWriteFileItem writer = StorageFactory.GetFileItemWriter();
                        Logger.LogInformation("File Scan GetFiles(): sending file to writer - {FilePath}", file.Path);
                        writer.Write(fileItem, false, true);
                    }
                });

                dbProjectId = repoWriter.Write(repoItem, dbProjectId, false);
            }
        }

        #endregion
    }
}
