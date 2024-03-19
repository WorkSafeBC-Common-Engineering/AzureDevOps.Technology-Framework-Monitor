using ProjectScanner;
using RepoScan.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class Scanner
    {
        #region Private Members

        private readonly Guid scanId = Guid.NewGuid();
        private readonly DateTime scanTime = DateTime.UtcNow;

        #endregion

        #region Public Methods

        public async Task ScanAsync(string projectId, string repositoryId)
        {
            IWriteRepoList writer = StorageFactory.GetRepoListWriter();

            Settings.Initialize();

            foreach (var name in Settings.Scanners)
            {
                var scanner = ScannerFactory.GetScanner(name);

                var organization = scanner.GetOrganization();

                await foreach (var project in scanner.Projects(projectId))
                {
                    bool repoOnly = false;

                    var repos = await scanner.Repositories(project, repositoryId);

                    foreach (var repo in repos)
                    {
                        var repoItem = new RepositoryItem
                        {
                            ScanID = scanId,
                            ScanTime = scanTime,
                            Source = organization.Source,
                            OrgName = organization.Name,
                            OrgUrl = organization.Url,
                            ProjectName = project.Name,
                            ProjectId = project.Id,
                            ProjectAbbreviation = project.Abbreviation,
                            ProjectDescription = project.Description,
                            ProjectLastUpdate = project.LastUpdate,
                            ProjectRevision = project.Revision,
                            ProjectState = project.State,
                            ProjectUrl = project.Url,
                            ProjectVisibility = project.Visibility,
                            RepositoryId = repo.Id,
                            RepositoryName = repo.Name,
                            RepositoryDefaultBranch = repo.DefaultBranch,
                            RepositoryIsFork = repo.IsFork,
                            RepositorySize = repo.Size,
                            RepositoryUrl = repo.Url,
                            RepositoryRemoteUrl = repo.RemoteUrl,
                            RepositoryWebUrl = repo.WebUrl,
                            RepositoryLastCommitId = repo.LastCommitId
                        };

                        // Write repo item to queue
                        writer.Write(repoItem, repoOnly);
                        repoOnly = true;
                    }

                    await PipelineScanner.ScanAsync(scanner, project.Id);
                }
            }
        }

        #endregion
    }
}
