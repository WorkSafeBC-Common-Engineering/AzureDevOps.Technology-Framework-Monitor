using ProjectData;
using ProjectData.Interfaces;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProjectScannerSaveToSqlServer
{
    public class Read : DbCore, IStorageReader
    {
        #region Private Members

        private Organization organization = null;

        #endregion

        #region IStorageReader Implementation

        void IStorageReader.Initialize(string configuration)
        {
            Initialize(configuration);
        }

        bool IStorageReader.IsDatabase
        {
            get { return true; }
        }

        Organization IStorageReader.GetOrganization()
        {
            var dbOrganization = GetOrganization();

            if (dbOrganization == null)
            {
                return null;
            }

            ProjectSource source = (ProjectSource)Enum.Parse(typeof(ProjectSource), dbOrganization.ScannerType.Value);
            organization = new Organization(source, dbOrganization.Name, dbOrganization.Uri);

            foreach(var dbProject in dbOrganization.Projects)
            {
                var dataProject = new Project
                {
                    Id = new Guid(dbProject.ProjectId),
                    Name = dbProject.Name,
                    Abbreviation = dbProject.Abbreviation,
                    Description = dbProject.Description,
                    LastUpdate = dbProject.LastUpdate ?? DateTime.MinValue,
                    Revision = dbProject.Revision,
                    State = dbProject.State,
                    Url = dbProject.Url,
                    Visibility = dbProject.Visibility,
                    Deleted = dbProject.Deleted,
                    NoScan = dbProject.NoScan
                };

                foreach (var repo in dbProject.Repositories)
                {
                    var repository = new Repository
                    {
                        Id = new Guid(repo.RepositoryId),
                        DefaultBranch = repo.DefaultBranch,
                        IsFork = repo.IsFork,
                        Name = repo.Name,
                        RemoteUrl = repo.RemoteUrl,
                        Size = repo.Size,
                        FileCount = repo.Files.Count,
                        Url = repo.Url,
                        WebUrl = repo.WebUrl,
                        Deleted = repo.Deleted,
                        LastCommitId = repo.LastCommitId,
                        NoScan = repo.NoScan                        
                    };

                    dataProject.AddRepository(repository);
                }

                organization.AddProject(dataProject);
            }

            return organization;
        }

        Project IStorageReader.GetProjectAndRepositories(string projectId, string repositoryId)
        {
            var dbProject = context.Projects
                                   .SingleOrDefaultAsync(p => p.ProjectId.Equals(projectId, StringComparison.InvariantCultureIgnoreCase)
                                                           && !p.Deleted)
                                   .Result;

            var project = new Project
            {
                Name = dbProject.Name,
                Id = new Guid(dbProject.ProjectId),
                Abbreviation = dbProject.Abbreviation,
                Description = dbProject.Description,
                LastUpdate = dbProject.LastUpdate ?? DateTime.MinValue,
                Revision = dbProject.Revision,
                State = dbProject.State,
                Url = dbProject.Url,
                Visibility = dbProject.Visibility,
                Deleted = dbProject.Deleted,
                NoScan = dbProject.NoScan
            };

            if (repositoryId == string.Empty)
            {
                foreach (var repo in dbProject.Repositories)
                {
                    var repository = new Repository
                    {
                        Id = new Guid(repo.RepositoryId),
                        DefaultBranch = repo.DefaultBranch,
                        IsFork = repo.IsFork,
                        Name = repo.Name,
                        RemoteUrl = repo.RemoteUrl,
                        Size = repo.Size,
                        FileCount = repo.Files.Count,
                        Url = repo.Url,
                        WebUrl = repo.WebUrl,
                        Deleted = repo.Deleted,
                        LastCommitId = repo.LastCommitId,
                        NoScan = repo.NoScan
                    };

                    project.AddRepository(repository);
                }
            }
            else
            {
                var repository = context.Repositories.SingleOrDefaultAsync(r => r.RepositoryId.Equals(repositoryId, StringComparison.InvariantCultureIgnoreCase)
                                                                             && !r.Deleted)
                                        .Result;
                
                if (repository != null)
                {
                    var repo = new Repository
                    {
                        DefaultBranch = repository.DefaultBranch,
                        Id = new Guid(repository.RepositoryId),
                        IsFork = repository.IsFork,
                        Name = repository.Name,
                        RemoteUrl = repository.RemoteUrl,
                        Size = repository.Size,
                        Url = repository.Url,
                        WebUrl = repository.WebUrl,
                        Deleted = repository.Deleted,
                        LastCommitId = repository.LastCommitId
                    };

                    project.AddRepository(repo);
                }
            }

            return project;
        }

        Repository IStorageReader.GetRepository()
        {
            throw new NotImplementedException();
        }

        Repository IStorageReader.GetRepository(Guid id)
        {
            Repository repository = null;
            var repoId = id.ToString("D");

            context.Database.CommandTimeout = 3600;
            var repo = context.Repositories
                           .SingleOrDefaultAsync(r => r.RepositoryId.Equals(repoId, StringComparison.InvariantCultureIgnoreCase)
                                                   && !r.Deleted && !r.Project.Deleted)
                           .Result;

            if (repo != null)
            {
                repository = new Repository
                {
                    DefaultBranch = repo.DefaultBranch,
                    Id = id,
                    IsFork = repo.IsFork,
                    Name = repo.Name,
                    RemoteUrl = repo.RemoteUrl,
                    Size = repo.Size,
                    Url = repo.Url,
                    WebUrl = repo.WebUrl,
                    OrgName = repo.Project.Organization.Name,
                    Deleted = repo.Deleted,
                    LastCommitId = repo.LastCommitId
                };
            }

            return repository;
        }

        IEnumerable<FileItem> IStorageReader.GetFiles()
        {
            context.Database.CommandTimeout = 300;
            foreach (var mainFile in context.Files)
            {
                FileItem fileItem;

                using (var localContext = GetConnection())
                {
                    context.Database.CommandTimeout = 300;
                    var file = localContext.Files.SingleOrDefault(f => f.Id == mainFile.Id);

                    var fileType = (FileItemType)Enum.Parse(typeof(FileItemType), file.FileType.Value);

                    fileItem = new FileItem
                    {
                        Id = file.FileId,
                        FileType = fileType,
                        Path = file.Path,
                        Url = file.Url,
                        StorageId = file.Id,
                        RepositoryId = new Guid(file.Repository.RepositoryId)
                    };

                    var pkgRefs = file.FileReferences
                                      .Where(pr => pr.FileReferenceTypeId == RefTypePkg)
                                      .Select(pr => new PackageReference
                                      {
                                          Id = pr.Name,
                                          PackageType = pr.PackageType,
                                          Version = pr.Version,  
                                          VersionComparator = pr.VersionComparator,
                                          FrameworkVersion = pr.FrameworkVersion
                                      });

                    fileItem.PackageReferences.AddRange(pkgRefs);

                    var refs = file.FileReferences
                                   .Where(r => r.FileReferenceTypeId == RefTypeFile)
                                   .Select(r => r.Name);

                    fileItem.References.AddRange(refs);

                    var urlRefs = file.FileReferences
                                      .Where(ur => ur.FileReferenceTypeId == RefTypeUrl)
                                      .Select(ur => new UrlReference
                                      {
                                          Url = ur.Name,
                                          Path = ur.Path
                                      });

                    fileItem.UrlReferences.AddRange(urlRefs);

                    var properties = file.FileProperties
                         .Where(p => p.FilePropertyType.Id == PropertyTypeProperty)
                         .Select(p => new { Key = p.Name, p.Value });

                    foreach (var property in properties)
                    {
                        fileItem.Properties.Add(property.Key, property.Value);
                    }

                    var filterItems = file.FileProperties
                                          .Where(fi => fi.PropertyTypeId == PropertyTypeFilteredItem)
                                          .Select(fi => new { Key = fi.Name, fi.Value });

                    foreach (var filterItem in filterItems)
                    {
                        fileItem.FilteredItems.Add(filterItem.Key, filterItem.Value);
                    }
                }

                yield return fileItem;
            }
        }

        IEnumerable<FileItem> IStorageReader.GetFiles(string id)
        {
            context.Database.CommandTimeout = 3600;
            var fileList = context.Files
                                  .Where(f => f.Repository.RepositoryId == id)
                                  .ToArray();

            foreach (var mainFile in fileList)
            {
                FileItem fileItem;

                var fileType = (FileItemType)Enum.Parse(typeof(FileItemType), mainFile.FileType.Value);

                fileItem = new FileItem
                {
                    Id = mainFile.FileId,
                    FileType = fileType,
                    Path = mainFile.Path,
                    Url = mainFile.Url,
                    StorageId = mainFile.Id,
                    RepositoryId = new Guid(mainFile.Repository.RepositoryId),
                    CommitId = mainFile.CommitId
                };

                context.Database.CommandTimeout = 3600;
                var pkgRefs = context.FileReferences
                                          .Where(fr => fr.FileId == mainFile.Id
                                                    && fr.FileReferenceTypeId == RefTypePkg)
                                          .Select(fr => new PackageReference
                                          {
                                              Id = fr.Name,
                                              PackageType = fr.PackageType,
                                              Version = fr.Version,
                                              VersionComparator = fr.VersionComparator,
                                              FrameworkVersion = fr.FrameworkVersion
                                          });

                fileItem.PackageReferences.AddRange(pkgRefs);

                context.Database.CommandTimeout = 3600;
                var refs = context.FileReferences
                                       .Where(fr => fr.FileId == mainFile.Id
                                                 && fr.FileReferenceTypeId == RefTypeFile)
                                       .Select(fr => fr.Name);

                fileItem.References.AddRange(refs);

                context.Database.CommandTimeout = 3600;
                var urlRefs = context.FileReferences
                                          .Where(fr => fr.FileId == mainFile.Id
                                                    && fr.FileReferenceTypeId == RefTypeUrl)
                                          .Select(fr => new UrlReference
                                          {
                                              Url = fr.Name,
                                              Path = fr.Path
                                          });

                fileItem.UrlReferences.AddRange(urlRefs);

                context.Database.CommandTimeout = 3600;
                var properties = context.FileProperties
                                             .Where(fp => fp.FileId == mainFile.Id
                                                       && fp.FilePropertyType.Id == PropertyTypeProperty)
                                             .Select(fp => new { Key = fp.Name, fp.Value });

                foreach (var property in properties)
                {
                    fileItem.Properties.Add(property.Key, property.Value);
                }

                context.Database.CommandTimeout = 3600;
                var filterItems = context.FileProperties
                                              .Where(fp => fp.FileId == mainFile.Id
                                                        && fp.PropertyTypeId == PropertyTypeFilteredItem)
                                              .Select(fp => new { Key = fp.Name, fp.Value });

                foreach (var filterItem in filterItems)
                {
                    fileItem.FilteredItems.Add(filterItem.Key, filterItem.Value);
                }

                yield return fileItem;
            }
        }

        void IStorageReader.Close()
        {
            Close();
        }

        #endregion

        #region Private Methods and Properties

        private DataModels.Organization GetOrganization()
        {
            context.Database.CommandTimeout = 300;
            var dbOrganization = context.Organizations
                                      .OrderBy(o => o.Id)
                                      .Where(o => o.Id > organizationId)
                                      .FirstOrDefault();

            organizationId = dbOrganization == null ? 0 : dbOrganization.Id;

            return dbOrganization;
        }

        #endregion
    }
}
