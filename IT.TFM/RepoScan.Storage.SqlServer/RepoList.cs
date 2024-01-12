using ProjectData.Interfaces;
using RepoScan.DataModels;
using DataStorage = Storage;
using System.Collections.Generic;
using System.Linq;
using ProjectData;
using System;

namespace RepoScan.Storage.SqlServer
{
    public class RepoList : IReadRepoList, IWriteRepoList
    {
        #region Private Members

        private IStorageWriter sqlWriter = null;

        private IStorageReader sqlReader = null;

        #endregion

        #region IReadRepoList Implementation

        IEnumerable<RepositoryItem> IReadRepoList.Read(string projectId, string repositoryId)
        {
            var reader = GetReader();
            var org = reader.GetOrganization();

            if (org == null)
            {
                yield break;
            }

            var projectList = projectId == string.Empty
                ? org.Projects
                : new Project[] { reader.GetProjectAndRepositories(projectId, repositoryId) };

            while (org != null && projectList != null && projectList.Any())
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
                            RepositoryUrl = repo.Url,
                            RepositoryWebUrl = repo.WebUrl,
                            IsDeleted = repo.Deleted,
                            RepositoryLastCommitId = repo.LastCommitId,
                            RepositoryNoScan = repo.NoScan
                        };

                        yield return item;
                    }
                }

                org = reader.GetOrganization();
            }
        }

        #endregion

        #region IWriteRepoList Implementation

        void IWriteRepoList.Write(RepositoryItem item, bool repoOnly)
        {
            var writer = GetWriter();

            if (!repoOnly)
            {
                var organization = new Organization(item.Source, item.OrgName, item.OrgUrl);
                writer.SaveOrganization(organization);

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

                writer.SaveProject(project);
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

            writer.SaveRepository(repository);
        }

        #endregion

        #region Private Methods

        private IStorageWriter GetWriter()
        {
            sqlWriter ??= DataStorage.StorageFactory.GetStorageWriter();

            return sqlWriter;
        }

        private IStorageReader GetReader()
        {
            sqlReader ??= DataStorage.StorageFactory.GetStorageReader();

            return sqlReader;
        }

        #endregion
    }
}
