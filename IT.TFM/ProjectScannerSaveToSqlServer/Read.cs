using ProjectData;
using ProjectData.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer
{
    public class Read : DbCore, IStorageReader
    {
        #region Private Members

        private Organization organization = null;
        private Project project = null;

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
                var project = new Project
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
                    Deleted = dbProject.Deleted
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
                        Url = repo.Url,
                        WebUrl = repo.WebUrl,
                        Deleted = repo.Deleted,
                    };

                    project.AddRepository(repository);
                }

                organization.AddProject(project);
            }

            return organization;
        }

        Project IStorageReader.GetProject()
        {
            throw new NotImplementedException();
        }

        Repository IStorageReader.GetRepository()
        {
            throw new NotImplementedException();
        }

        Repository IStorageReader.GetRepository(Guid id)
        {
            Repository repository = null;
            var repoId = id.ToString("D");

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
                    Deleted = repo.Deleted
                };
            }

            return repository;
        }

        IEnumerable<FileItem> IStorageReader.GetFiles()
        {
            foreach (var mainFile in context.Files)
            {
                FileItem fileItem;

                using (var localContext = GetConnection())
                {
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
            var fileList = context.Files.Where(f => f.Repository.RepositoryId == id);

            foreach (var mainFile in fileList)
            {
                FileItem fileItem;

                using (var localContext = GetConnection())
                {
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

        void IStorageReader.Close()
        {
            Close();
        }

        #endregion

        #region Private Methods and Properties

        private DataModels.Organization GetOrganization()
        {
            var dbOrganization = context.Organizations
                                      .OrderBy(o => o.Id)
                                      .Where(o => o.Id > organizationId)
                                      .FirstOrDefault();

            organizationId = dbOrganization == null ? 0 : dbOrganization.Id;

            return dbOrganization;
        }

        private FileItem GetFile(DataModels.File dbFile)
        {
            var fileType = (FileItemType)Enum.Parse(typeof(FileItemType), dbFile.FileType.Value);

            var file = new FileItem
            {
                Id = dbFile.FileId,
                FileType = fileType,
                Path = dbFile.Path,
                Url = dbFile.Url,
            };

            foreach (var pkgRef in dbFile.FileReferences
                                         .Where(r => r.FileReferenceTypeId == RefTypePkg)
                                         .Select(r => new PackageReference
                                         {
                                            Id = r.Name,
                                            PackageType = r.PackageType,
                                            Version = r.Version,
                                            VersionComparator =  r.VersionComparator,
                                            FrameworkVersion = r.FrameworkVersion
                                         }))
            {
                file.PackageReferences.Add(pkgRef);
            }

            foreach (var fileRef in dbFile.FileReferences
                                          .Where(r => r.FileReferenceTypeId == RefTypeFile)
                                          .Select(r => r.Name))
            {
                file.References.Add(fileRef);
            }

            foreach (var urlRef in dbFile.FileReferences
                                         .Where(r => r.FileReferenceTypeId == RefTypeUrl)
                                         .Select(r => new UrlReference
                                         {
                                             Path = r.Path,
                                             Url = r.Name
                                         }))
            {
                file.UrlReferences.Add(urlRef);
            }

            foreach (var property in dbFile.FileProperties
                                       .Where(r => r.PropertyTypeId == PropertyTypeProperty)
                                       .Select (r => new KeyValuePair<string, string>(r.Name, r.Value)))
            {
                file.Properties.Add(property.Key, property.Value);
            }

            foreach (var item in dbFile.FileProperties
                                       .Where(r => r.PropertyTypeId == PropertyTypeProperty)
                                       .Select(r => new KeyValuePair<string, string>(r.Name, r.Value)))
            {
                file.FilteredItems.Add(item.Key, item.Value);
            }

            return file;
        }

        private IEnumerable<Project> GetProjects()
        {
            foreach (var p in organization.Projects)
            {
                project = p;
                yield return p;
            }
        }

        private IEnumerable<Repository> GetRepositories()
        {
            foreach (var r in project.Repositories)
            {
                yield return r;
            }
        }

        #endregion
    }
}
