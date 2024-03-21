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
        private int yamlFileType = 10;

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

        Organization IStorageReader.GetOrganization(string projectId, string repositoryId)
        {
            var dbOrganization = GetOrganization();

            if (dbOrganization == null)
            {
                return null;
            }

            ProjectSource source = (ProjectSource)Enum.Parse(typeof(ProjectSource), dbOrganization.ScannerType.Value);
            organization = new Organization(source, dbOrganization.Name, dbOrganization.Uri);

            Project project;

            if (projectId == string.Empty)
            {
                foreach (var dataProject in dbOrganization.Projects)
                {
                    project = new Project
                    {
                        Id = new Guid(dataProject.ProjectId),
                        Name = dataProject.Name,
                        Abbreviation = dataProject.Abbreviation,
                        Description = dataProject.Description,
                        LastUpdate = dataProject.LastUpdate ?? DateTime.MinValue,
                        Revision = dataProject.Revision,
                        State = dataProject.State,
                        Url = dataProject.Url,
                        Visibility = dataProject.Visibility,
                        Deleted = dataProject.Deleted,
                        NoScan = dataProject.NoScan
                    };

                    GetRepositoriesForProject(dataProject, project);

                    organization.AddProject(project);
                }
            }

            else
            {
                var dbProject = context.Projects
                                       .SingleOrDefaultAsync(p => p.ProjectId.Equals(projectId, StringComparison.InvariantCultureIgnoreCase)
                                                               && !p.Deleted)
                                       .Result;

                project = new Project
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

                if (repositoryId != string.Empty)
                {
                    var repository = context.Repositories.SingleOrDefaultAsync(r => r.RepositoryId.Equals(repositoryId, StringComparison.InvariantCultureIgnoreCase)
                                                                                 && !r.Deleted)
                                            .Result;

                    if (repository != null)
                    {
                        var repo = new Repository
                        {
                            Id = new Guid(repository.RepositoryId),
                            DefaultBranch = repository.DefaultBranch,
                            IsFork = repository.IsFork,
                            Name = repository.Name,
                            RemoteUrl = repository.RemoteUrl,
                            Size = repository.Size,
                            FileCount = repository.Files.Count,
                            PipelineCount = repository.Pipelines.Count,
                            Url = repository.Url,
                            WebUrl = repository.WebUrl,
                            Deleted = repository.Deleted,
                            LastCommitId = repository.LastCommitId,
                            NoScan = repository.NoScan
                        };

                        project.AddRepository(repo);
                    }
                }

                else
                {
                    GetRepositoriesForProject(dbProject, project);
                }

                organization.AddProject(project);
            }

            return organization;
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
                    LastCommitId = repo.LastCommitId,
                    NoScan = repo.NoScan
                };
            }

            return repository;
        }

        IEnumerable<string> IStorageReader.GetRepositoryIds()
        {
            return context.Repositories
                          .Select(r => r.RepositoryId)
                          .ToArray()
                          .AsEnumerable();
        }

        IEnumerable<FileItem> IStorageReader.GetFiles()
        {
            context.Database.CommandTimeout = 300;
            foreach (var mainFile in context.Files)
            {
                FileItem fileItem;

                using (var localContext = GetConnection())
                {
                    localContext.Database.CommandTimeout = 300;
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
                                          })
                                          .ToArray();

                fileItem.PackageReferences.AddRange(pkgRefs);

                context.Database.CommandTimeout = 3600;
                var refs = context.FileReferences
                                       .Where(fr => fr.FileId == mainFile.Id
                                                 && fr.FileReferenceTypeId == RefTypeFile)
                                       .Select(fr => fr.Name)
                                       .ToArray();

                fileItem.References.AddRange(refs);

                context.Database.CommandTimeout = 3600;
                var urlRefs = context.FileReferences
                                          .Where(fr => fr.FileId == mainFile.Id
                                                    && fr.FileReferenceTypeId == RefTypeUrl)
                                          .Select(fr => new UrlReference
                                          {
                                              Url = fr.Name,
                                              Path = fr.Path
                                          })
                                          .ToArray();

                fileItem.UrlReferences.AddRange(urlRefs);

                context.Database.CommandTimeout = 3600;
                var properties = context.FileProperties
                                             .Where(fp => fp.FileId == mainFile.Id
                                                       && fp.FilePropertyType.Id == PropertyTypeProperty)
                                             .Select(fp => new { Key = fp.Name, fp.Value })
                                             .ToArray();

                foreach (var property in properties)
                {
                    fileItem.Properties.Add(property.Key, property.Value);
                }

                context.Database.CommandTimeout = 3600;
                var filterItems = context.FileProperties
                                              .Where(fp => fp.FileId == mainFile.Id
                                                        && fp.PropertyTypeId == PropertyTypeFilteredItem)
                                              .Select(fp => new { Key = fp.Name, fp.Value })
                                              .ToArray();

                foreach (var filterItem in filterItems)
                {
                    fileItem.FilteredItems.Add(filterItem.Key, filterItem.Value);
                }

                yield return fileItem;
            }
        }

        IEnumerable<FileItem> IStorageReader.GetYamlFiles(string id)
        {
            context.Database.CommandTimeout = 3600;
            var fileList = context.Files
                                  .Where(f => f.Repository.RepositoryId == id
                                           && f.FileTypeId == yamlFileType)
                                  .ToArray();

            foreach (var mainFile in fileList)
            {
                FileItem fileItem;

                fileItem = new FileItem
                {
                    Id = mainFile.FileId,
                    FileType = FileItemType.YamlPipeline,
                    Path = mainFile.Path,
                    Url = mainFile.Url,
                    StorageId = mainFile.Id,
                    RepositoryId = new Guid(mainFile.Repository.RepositoryId),
                    CommitId = mainFile.CommitId
                };

                yield return fileItem;
            }
        }

        IEnumerable<Pipeline> IStorageReader.GetPipelines(string pipelineType)
        {
            if (pipelineType != Pipeline.pipelineTypeYaml && pipelineType != Pipeline.pipelineTypeClassic)
            {
                throw new ArgumentOutOfRangeException(nameof(pipelineType), $"Invalid pipeline type specified ({pipelineType}). Must be one of ({Pipeline.pipelineTypeYaml}, {Pipeline.pipelineTypeClassic}).");
            }

            context.Database.CommandTimeout = 300;
            var pipelines = context.Pipelines.Where(p => p.Type == pipelineType);
            foreach (var dbPipeline in pipelines)
            {
                var file = context.Files.SingleOrDefault(f => f.Id == dbPipeline.FileId);

                Pipeline pipeline = new()
                {
                    Id = dbPipeline.Id,
                    ProjectId = dbPipeline?.Project?.ProjectId,
                    RepositoryId = dbPipeline?.Repository?.RepositoryId,
                    Name = dbPipeline.Name,
                    Folder = dbPipeline.Folder,
                    Revision = dbPipeline.Revision,
                    Url = dbPipeline.Url,
                    Type = dbPipeline.Type,
                    PipelineType = dbPipeline.Type,
                    Path = dbPipeline.Path,
                    FileId = file?.FileId
                };

                yield return pipeline;
            }
        }

        public IEnumerable<Pipeline> GetPipelines(string repositoryId, string fileId, string filePath)
        {
            var repository = context.Repositories.Single(r => r.RepositoryId == repositoryId);
            var file = context.Files.Single(f => f.RepositoryId == repository.Id
                                              && f.FileId.Equals(fileId, StringComparison.InvariantCultureIgnoreCase)
                                              && f.Path.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));

            var pipelines = context.Pipelines.Where(p => p.RepositoryId == repository.Id
                                                      && p.FileId == file.Id);

            var projectId = repository.Project.ProjectId;

            return pipelines.Select(p => new Pipeline
            { 
                Id = p.PipelineId,
                ProjectId = projectId,
                RepositoryId = p.Repository.RepositoryId,
                FileId = file.FileId,
                Name = p.Name,
                Folder = p.Folder,
                Revision = p.Revision,
                Url = p.Url,
                Type = p.Type,
                PipelineType = p.PipelineType,
                Path= p.Path,
                YamlType = p.YamlType,
                Portfolio = p.Portfolio,
                Product = p.Product
            }).AsEnumerable();
        }

        IEnumerable<int> IStorageReader.GetPipelineIdsForProject(string projectId)
        {
            var project = context.Projects.SingleOrDefault(p => p.ProjectId.Equals(projectId, StringComparison.InvariantCultureIgnoreCase));

            return context.Pipelines.Where(p => p.ProjectId == project.Id)
                                    .Select(p => p.PipelineId)
                                    .AsEnumerable();
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

        private static void GetRepositoriesForProject(DataModels.Project dataProject, Project project)
        {
            foreach (var repo in dataProject.Repositories)
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

        #endregion
    }
}
