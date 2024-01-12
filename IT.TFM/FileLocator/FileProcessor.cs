using ProjectData;
using ProjectData.Interfaces;
using ProjectScanner;
using RepoScan.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class FileProcessor
    {
        public static async Task GetFiles(int totalThreads, bool forceDetails, string projectId, string repositoryId)
        {
            Settings.Initialize();

            IReadRepoList reader = StorageFactory.GetRepoListReader();
            IWriteRepoList repoWriter = StorageFactory.GetRepoListWriter();
            IReadFileItem fileReader = StorageFactory.GetFileItemReader();

            var orgName = string.Empty;
            IScanner scanner = null;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            IEnumerable<Guid> projectList = new List<Guid>().AsEnumerable();

            var lastProject = Guid.Empty;
            IEnumerable<Repository> repoList = null;

            foreach (var repoItem in reader.Read(projectId, repositoryId))
            {
                // Skip any repos that have been flagged as deleted or No Scan
                if (repoItem.ProjectNoScan || repoItem.ProjectIsDeleted || repoItem.RepositoryNoScan || repoItem.IsDeleted)
                {
#if DEBUG
                    Console.WriteLine($"=> File Scan GetFiles(): Skipping repo {repoItem.RepositoryName} - ProjectNoScan = {repoItem.ProjectNoScan}, ProjectIsDeleted = {repoItem.ProjectIsDeleted}, RepositoryNoScan = {repoItem.RepositoryNoScan}, IsDeleted = {repoItem.IsDeleted}");
#endif
                    continue;
                }

                if (orgName != repoItem.OrgName)
                {
                    orgName = repoItem.OrgName;
                    scanner = ScannerFactory.GetScanner(orgName);

                    List<Guid> pList = [];
                    await foreach (var p in scanner.Projects(string.Empty))
                    {
                        pList.Add(p.Id);
                    }
                    projectList = pList.ToArray();
#if DEBUG
                    Console.WriteLine($"=> File Scan, GetFiles(): Get Project IDs returned {projectList.Count()} items");
#endif
                }

                // Check whether the project or repo still exists - it might have been moved or deleted.
                if (!projectList.Any(p => p == repoItem.ProjectId))
                {
                    repoItem.ProjectIsDeleted = true;
                    repoWriter.Write(repoItem, false);
#if DEBUG
                    Console.WriteLine($"=> File Scan GetFiles(): Project {repoItem.ProjectName} was deleted");
#endif
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
#if DEBUG
                    Console.WriteLine($"=> File Scan GetFiles(): Project {repoItem.ProjectName} returned {repoList.Count()} repositories");
#endif
                }

                bool repoExists = false;
                bool repoUnchanged = false;

                var r = repoList?.FirstOrDefault(r => r.Id == repoItem.RepositoryId);

                if (r != null)
                {
                    if (r.LastCommitId == repoItem.RepositoryLastCommitId)
                    {
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
                    repoWriter.Write(repoItem, false);
#if DEBUG
                    Console.WriteLine($"=> File Scan GetFiles(): repository {repoItem.RepositoryName} was deleted");
#endif
                    continue;
                }

                if (repoUnchanged && !forceDetails)
                {
#if DEBUG
                    Console.WriteLine($"=> File Scan GetFiles(): repository {repoItem.RepositoryName} is unchanged");
#endif
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
                    //if (!file.RepositoryId.Equals(new Guid("1aea9a89-b095-4184-8e07-8445fad0f0b9")))
                    //{
                    //    return;
                    //}
#if DEBUG
                    Console.WriteLine($"=> File Scan GetFiles(): Getting File: Repository = {repoItem.RepositoryName}, File Path = {file.Path}");
#endif

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
#if DEBUG
                        Console.WriteLine($"=> File Scan GetFiles(): sending file to writer - ${file.Path}");
#endif
                        writer.Write(fileItem, false, true);
                    }
                });

                repoWriter.Write(repoItem, false);
            }
        }
    }
}
