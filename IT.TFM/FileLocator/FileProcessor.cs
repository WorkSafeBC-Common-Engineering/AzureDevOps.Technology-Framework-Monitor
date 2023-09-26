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
        public async Task GetFiles(int totalThreads)
        {
            Settings.Initialize();

            IReadRepoList reader = StorageFactory.GetRepoListReader();
            IWriteFileItem writer = StorageFactory.GetFileItemWriter();
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

            foreach (var repoItem in reader.Read())
            {
                // Skip any repos that have been flagged as deleted or No Scan
                if (repoItem.ProjectNoScan || repoItem.ProjectIsDeleted || repoItem.RepositoryNoScan || repoItem.IsDeleted)
                {
                    continue;
                }

                if (orgName != repoItem.OrgName)
                {
                    orgName = repoItem.OrgName;
                    scanner = ScannerFactory.GetScanner(orgName);

                    List<Guid> pList = new();
                    await foreach (var p in scanner.Projects())
                    {
                        pList.Add(p.Id);
                    }
                    projectList = pList.ToArray();
                }

                // Check whether the project or repo still exists - it might have been moved or deleted.
                if (!projectList.Any(p => p == repoItem.ProjectId))
                {
                    repoItem.ProjectIsDeleted = true;
                    repoWriter.Write(repoItem, false);

                    continue;
                }

                if (lastProject != repoItem.ProjectId)
                {
                    var project = new Project
                    {
                        Id = repoItem.ProjectId,
                        Name = repoItem.ProjectName
                    };

                    repoList = await scanner.Repositories(project);
                    lastProject = repoItem.ProjectId;
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

                    continue;
                }

                if (repoUnchanged)
                {
                    continue;
                }

                repoWriter.Write(repoItem, false);

                var repo = new Repository
                {
                    Id = repoItem.RepositoryId,
                    DefaultBranch = repoItem.RepositoryDefaultBranch
                };

                var fileList = await scanner.Files(repoItem.ProjectId, repo);

                Parallel.ForEach(fileList, options, (file) =>
                {
                    if (!file.RepositoryId.Equals(new Guid("1aea9a89-b095-4184-8e07-8445fad0f0b9")))
                    {
                        return;
                    }

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

                        writer.Write(fileItem, false, true);
                    }
                });


                var repoIds = fileList.Select(f => f.RepositoryId.ToString()).Distinct();
                Parallel.ForEach(repoIds, options, (repoId) =>
                {
                    // Here we want to interate verify whether the file exists in the database but not in the fileList.
                    // If this is the case, the file was moved or deleted, and the one the database should be removed.
                    var dbFiles = fileReader.Read(repoId);
                    foreach (var dbFile in dbFiles)
                    {
                        if (!fileList.Any(f => f.Id.Equals(dbFile.Id)))
                        {
                            writer.Delete(dbFile);
                        }
                    }
                });
            }
        }
    }
}
