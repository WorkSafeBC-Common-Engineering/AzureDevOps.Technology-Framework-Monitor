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
        public void GetFiles(int totalThreads)
        {
            Settings.Initialize();

            IReadRepoList reader = StorageFactory.GetRepoListReader();
            IWriteFileItem writer = StorageFactory.GetFileItemWriter();
            IWriteRepoList repoWriter = StorageFactory.GetRepoListWriter();

            var orgName = string.Empty;
            IScanner scanner = null;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            IEnumerable<Guid> projectList = new List<Guid>().AsEnumerable();

            foreach (var repoItem in reader.Read())
            {
                // Skip any repos that have been flagged as deleted
                if (repoItem.ProjectIsDeleted || repoItem.IsDeleted)
                {
                    continue;
                }

                if (orgName != repoItem.OrgName)
                {
                    orgName = repoItem.OrgName;
                    scanner = ScannerFactory.GetScanner(orgName);
                    projectList = scanner.Projects().Select(p => p.Id).ToArray();
                }

                // Check whether the project or repo still exists - it might have been moved or deleted.
                if (!projectList.Any(p => p == repoItem.ProjectId))
                {
                    repoItem.ProjectIsDeleted = true;
                    repoWriter.Write(repoItem);

                    continue;
                }

                var project = new Project
                {
                    Id = repoItem.ProjectId,
                    Name = repoItem.ProjectName
                };

                var repoList = scanner.Repositories(project);
                bool repoExists = repoList.Any(r => r.Id == repoItem.RepositoryId);

                if (!repoExists)
                {
                    // flag repo as deleted.
                    repoItem.IsDeleted = true;
                    repoWriter.Write(repoItem);

                    continue;
                }

                var repo = new Repository
                {
                    Id = repoItem.RepositoryId,
                    DefaultBranch = repoItem.RepositoryDefaultBranch
                };

                Parallel.ForEach(scanner.Files(repoItem.ProjectId, repo, false), options, (file) =>
                {
                    if (file.FileType != FileItemType.NoMatch || FileFiltering.Filter.CanFilterFile(file))
                    {
                        var fileItem = new DataModels.FileItem
                        {
                            Repository = repoItem,
                            FileType = file.FileType,
                            Id = file.Id,
                            Path = file.Path,
                            Url = file.Url,
                            SHA1 = file.SHA1
                        };

                        writer.Write(fileItem, false, true);
                    }
                });
            }
        }
    }
}
