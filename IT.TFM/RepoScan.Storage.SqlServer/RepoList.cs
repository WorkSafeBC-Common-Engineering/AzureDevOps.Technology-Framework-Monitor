using ProjectData.Interfaces;
using RepoScan.DataModels;
using DataStorage = Storage;
using System.Collections.Generic;
using System.Linq;
using ProjectData;

namespace RepoScan.Storage.SqlServer
{
    public class RepoList : IReadRepoList, IWriteRepoList
    {
        #region IReadRepoList Implementation

        IEnumerable<RepositoryItem> IReadRepoList.Read(string projectId, string repositoryId)
        {
            using var reader = GetReader();
            var org = reader.GetOrganization(projectId, repositoryId);

            if (org == null)
            {
                yield break;
            }

            while (org != null && org.Projects != null && org.Projects.Any())
            {
                foreach (var project in org.Projects)
                {
                    if (!project.Repositories.Any())
                    {
                        continue;
                    }

                    foreach (var repo in project.Repositories)
                    {
                        var item = new RepositoryItem
                        {
                            Source = org.Source,
                            OrgName = org.Name,
                            OrgUrl = org.Url,

                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            ProjectAbbreviation = project.Abbreviation,
                            ProjectDescription = project.Description,
                            ProjectLastUpdate = project.LastUpdate,
                            ProjectRevision = project.Revision,
                            ProjectState = project.State,
                            ProjectUrl = project.Url,
                            ProjectVisibility = project.Visibility,
                            ProjectIsDeleted = project.Deleted,
                            ProjectNoScan = project.NoScan,

                            RepositoryDefaultBranch = repo.DefaultBranch,
                            RepositoryId = repo.Id,
                            RepositoryIsFork = repo.IsFork,
                            RepositoryName = repo.Name,
                            RepositoryRemoteUrl = repo.RemoteUrl,
                            RepositorySize = repo.Size,
                            RepositoryTotalFiles = repo.FileCount,
                            RepositoryTotalPipelines = repo.PipelineCount,
                            RepositoryUrl = repo.Url,
                            RepositoryWebUrl = repo.WebUrl,
                            IsDeleted = repo.Deleted,
                            RepositoryLastCommitId = repo.LastCommitId,
                            RepositoryNoScan = repo.NoScan
                        };

                        yield return item;
                    }
                }

                org = reader.GetOrganization(projectId, repositoryId);
            }
        }

        IEnumerable<string> IReadRepoList.GetRepositoryIds()
        {
            using var reader = GetReader();
            return reader.GetRepositoryIds();
        }

        #endregion

        #region IWriteRepoList Implementation

        int IWriteRepoList.Write(RepositoryItem item, int projectId, bool repoOnly)
        {
            using var writer = GetWriter();
            int organizationId = 0;

            if (!repoOnly)
            {
                var organization = new Organization(item.Source, item.OrgName, item.OrgUrl);
                organizationId = writer.SaveOrganization(organization);

                var project = new Project()
                {
                    Id = item.ProjectId,
                    Url = item.ProjectUrl,
                    Abbreviation = item.ProjectAbbreviation,
                    Description = item.ProjectDescription,
                    LastUpdate = item.ProjectLastUpdate,
                    Name = item.ProjectName,
                    Revision = item.ProjectRevision,
                    State = item.ProjectState,
                    Visibility = item.ProjectVisibility,
                    Deleted = item.ProjectIsDeleted
                };

                projectId = writer.SaveProject(project, organizationId);
            }

            var repository = new Repository
            {
                Id = item.RepositoryId,
                DefaultBranch = item.RepositoryDefaultBranch,
                IsFork = item.RepositoryIsFork,
                Name = item.RepositoryName,
                RemoteUrl = item.RepositoryRemoteUrl,
                Size = item.RepositorySize,
                Url = item.RepositoryUrl,
                WebUrl = item.RepositoryWebUrl,
                Deleted = item.IsDeleted,
                LastCommitId = item.RepositoryLastCommitId
            };

            _ = writer.SaveRepository(repository, projectId);

            return projectId;
        }

        #endregion

        #region Private Methods

        private static IStorageWriter GetWriter()
        {
            return DataStorage.StorageFactory.GetStorageWriter();
        }

        private static IStorageReader GetReader()
        {
            return DataStorage.StorageFactory.GetStorageReader();
        }

        #endregion
    }
}
