using ProjectData;
using ProjectData.Interfaces;

using DbContext = ProjectScannerSaveToSqlServer.DataModels.ProjectScannerDB;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer
{
    public class Read : DbCore, IStorageReader
    {
        #region Private Members

        private Organization organization = null;
        private const int yamlFileType = 10;

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
                var dbProject = _compiledProjectQueryByProjectId(context, projectId).Result;

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
                    var repository = _compiledRepositoryQueryByRepositoryId(context, repositoryId).Result;

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

            context.Database.SetCommandTimeout(3600);
            var repo = _compiledRepositoryQueryByRepositoryIdNotDeleted(context, repoId).Result;

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
                    NoScan = repo.NoScan,
                    CreatedOn = repo.CreatedOn,
                    LastUpdatedOn = repo.LastUpdatedOn
                };
            }

            return repository;
        }

        IEnumerable<string> IStorageReader.GetRepositoryIds()
        {
            return _compiledRepositoryIds(context).ToBlockingEnumerable().ToArray();
        }

        IEnumerable<FileItem> IStorageReader.GetFiles()
        {
            context.Database.SetCommandTimeout(300);
            foreach (var mainFile in _compiledFiles(context).ToBlockingEnumerable().ToArray())
            {
                FileItem fileItem;

                using (var localContext = GetConnection())
                {
                    localContext.Database.SetCommandTimeout(300);
                    var file = _compiledFilesById(localContext, mainFile.Id).Result;

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
            var files = new List<FileItem>();
            context.Database.SetCommandTimeout(3600);
            var fileList = context.Files.Where(f => f.Repository.RepositoryId == id).ToArray();

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

                context.Database.SetCommandTimeout(3600);
                var pkgRefs = _compiledFileReferencesByFileId(context, mainFile.Id, RefTypePkg).ToBlockingEnumerable()
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

                context.Database.SetCommandTimeout(3600);
                var refs = _compiledFileReferencesByFileId(context, mainFile.Id, RefTypeFile).ToBlockingEnumerable()
                                                                                             .Select(fr => fr.Name)
                                                                                             .ToArray();
                fileItem.References.AddRange(refs);

                context.Database.SetCommandTimeout(3600);
                var urlRefs = _compiledFileReferencesByFileId(context, mainFile.Id, RefTypeUrl).ToBlockingEnumerable()
                                                                                               .Select(fr => new UrlReference
                                                                                               {
                                                                                                   Url = fr.Name,
                                                                                                   Path = fr.Path
                                                                                               })
                                                                                               .ToArray();
                fileItem.UrlReferences.AddRange(urlRefs);

                context.Database.SetCommandTimeout(3600);
                var properties = _compiledFilePropertiesByFileId(context, mainFile.Id, PropertyTypeProperty).ToBlockingEnumerable()
                                                                                                            .Select(fp => new { Key = fp.Name, fp.Value })
                                                                                                            .ToArray();
                foreach (var property in properties)
                {
                    fileItem.Properties.Add(property.Key, property.Value);
                }

                context.Database.SetCommandTimeout(3600);
                var filterItems = _compiledFilePropertiesByFileId(context, mainFile.Id, PropertyTypeFilteredItem).ToBlockingEnumerable()
                                                                                                                 .Select(fp => new { Key = fp.Name, fp.Value })
                                                                                                                 .ToArray();
                foreach (var filterItem in filterItems)
                {
                    fileItem.FilteredItems.Add(filterItem.Key, filterItem.Value);
                }

                files.Add(fileItem);
                //yield return fileItem;
            }
            return files.AsEnumerable();
        }

        IEnumerable<FileItem> IStorageReader.GetYamlFiles(string id)
        {
            context.Database.SetCommandTimeout(3600);
            var fileList = _compiledGetYamlFilesByRepositoryId(context, id).ToBlockingEnumerable().ToArray();

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

            var pipelineList = new List<Pipeline>();

            context.Database.SetCommandTimeout(300);
            var pipelines = _compiledGetPipelinesByType(context, pipelineType).ToBlockingEnumerable().ToArray();

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

                pipelineList.Add(pipeline);
            }

            return pipelineList.AsEnumerable();
        }

        IEnumerable<Pipeline> IStorageReader.GetPipelines(string repositoryId, string filePath)
        {
            var repository = _compiledRepositoryQueryByRepositoryId(context, repositoryId).Result;
            var projectId = repository.Project.ProjectId;

            var file = _compiledFileByRepositoryAndPath(context, repositoryId, filePath).Result;

            var pipelines = _compiledGetPipelinesByRepositoryAndFile(context, repositoryId, file.Id).ToBlockingEnumerable().ToArray();

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
                Path = p.Path,
                YamlType = p.YamlType,
                BlueprintApplicationType = p.BlueprintType.Value,
                SuppressCD = p.SuppressCD,
                Portfolio = p.Portfolio,
                Product = p.Product
            }).AsEnumerable();
        }

        IEnumerable<int> IStorageReader.GetPipelineIdsForProject(string projectId)
        {
            var project = _compiledProjectQueryByProjectId(context, projectId).Result;

            return _compiledGetPipelineIdsByProject(context, project.Id).ToBlockingEnumerable().ToArray();
        }

        IEnumerable<NuGetFeed> IStorageReader.GetNuGetFeeds()
        {
            var feeds = _compiledGetNuGetFeeds(context).ToBlockingEnumerable();
            return feeds.Select(f => new NuGetFeed
            {
                Name = f.Name,
                FeedUrl = f.FeedUrl,
                ProjectId = f.Project?.ProjectId
            }).AsEnumerable();
        }

        IEnumerable<FileItem> IStorageReader.GetFilesWithProperties(FileItemType fileType, string propertyId, string propertyValue)
        {
            var properties = context.FileProperties
                                    .Where(p => p.File.FileType.Value == fileType.ToString());

            if (!string.IsNullOrEmpty(propertyValue))
            {
                properties = properties.Where(p => p.Name == propertyId);

                if (!string.IsNullOrEmpty(propertyValue))
                { 
                    properties = properties.Where(p => p.Value == propertyValue);
                }
            }

            var files = properties.Select(p => p.File)
                                  .Distinct()
                                  .Select(f => new FileItem
                                  {
                                      Id = f.FileId,
                                      FileType = fileType,
                                      Path = f.Path,
                                      Url = f.Url,
                                      StorageId = f.Id,
                                      RepositoryId = new Guid(f.Repository.RepositoryId),                                      
                                      CommitId = f.CommitId
                                  })
                                  .ToArray();

            return files.AsEnumerable();            
        }

        #endregion

        #region Private Methods and Properties

        private DataModels.Organization GetOrganization()
        {
            context.Database.SetCommandTimeout(300);

            var dbOrganization = _compiledOrganizationQuery(context, organizationId).Result;

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

        #region Compiled Link Queries

        private static readonly Func<DbContext, int, Task<DataModels.Organization>> _compiledOrganizationQuery
            = EF.CompileAsyncQuery((DbContext context, int orgId) => context.Organizations
                                                                            .OrderBy(o => o.Id)
                                                                            .Where(o => o.Id > orgId)
                                                                            .FirstOrDefault());

        private static readonly Func<DbContext, string, Task<DataModels.Project>> _compiledProjectQueryByProjectId
            = EF.CompileAsyncQuery((DbContext context, string projectId) => context.Projects
                                                                                   .SingleOrDefault(p => p.ProjectId == projectId));

        private static readonly Func<DbContext, string, Task<DataModels.Repository>> _compiledRepositoryQueryByRepositoryId
            = EF.CompileAsyncQuery((DbContext context, string repositoryId) => context.Repositories
                                                                                      .SingleOrDefault(r => r.RepositoryId == repositoryId));

        private static readonly Func<DbContext, string, Task<DataModels.Repository>> _compiledRepositoryQueryByRepositoryIdNotDeleted
            = EF.CompileAsyncQuery((DbContext context, string repositoryId) => context.Repositories
                                                                                      .SingleOrDefault(r => r.RepositoryId == repositoryId
                                                                                                         && !r.Deleted && !r.Project.Deleted));

        private static readonly Func<DbContext, IAsyncEnumerable<string>> _compiledRepositoryIds
            = EF.CompileAsyncQuery((DbContext context) => context.Repositories
                                                                 .Select(r => r.RepositoryId));

        private static readonly Func<DbContext, IAsyncEnumerable<DataModels.File>> _compiledFiles
            = EF.CompileAsyncQuery((DbContext context) => context.Files);

        private static readonly Func<DbContext, int, Task<DataModels.File>> _compiledFilesById
            = EF.CompileAsyncQuery((DbContext context, int fileId) => context.Files
                                                                             .SingleOrDefault(f => f.Id == fileId));

        private static readonly Func<DbContext, string, IAsyncEnumerable<DataModels.File>> _compiledFilesByRepositoryId
            = EF.CompileAsyncQuery((DbContext context, string repositoryId) => context.Files
                                                                                      .Where(f => f.Repository.RepositoryId == repositoryId));

        private static readonly Func<DbContext, string, string, Task<DataModels.File>> _compiledFileByRepositoryAndPath
            = EF.CompileAsyncQuery((DbContext context, string repositoryId, string filePath) => context.Files
                                                                                                       .SingleOrDefault(f => f.Repository.RepositoryId == repositoryId
                                                                                                                          && f.Path == filePath));

        private static readonly Func<DbContext, int, int, IAsyncEnumerable<DataModels.FileReference>> _compiledFileReferencesByFileId
            = EF.CompileAsyncQuery((DbContext context, int fileId, int fileRefType) => context.FileReferences
                                                                                              .Where(fr => fr.FileId == fileId
                                                                                                        && fr.FileReferenceTypeId == fileRefType));

        private static readonly Func<DbContext, int, int, IAsyncEnumerable<DataModels.FileProperty>> _compiledFilePropertiesByFileId
            = EF.CompileAsyncQuery((DbContext context, int fileId, int filePropertyType) => context.FileProperties
                                                                                                   .Where(fp => fp.FileId == fileId
                                                                                                             && fp.PropertyTypeId == filePropertyType));

        private static readonly Func<DbContext, string, IAsyncEnumerable<DataModels.File>> _compiledGetYamlFilesByRepositoryId
            = EF.CompileAsyncQuery((DbContext context, string repositoryId) => context.Files
                                                                                      .Where(f => f.Repository.RepositoryId == repositoryId
                                                                                               && f.FileTypeId == yamlFileType));

        private static readonly Func<DbContext, string, IAsyncEnumerable<DataModels.Pipeline>> _compiledGetPipelinesByType
            = EF.CompileAsyncQuery((DbContext context, string pipelineType) => context.Pipelines
                                                                                      .Where(p => p.Type == pipelineType));

        private static readonly Func<DbContext, string, int, IAsyncEnumerable<DataModels.Pipeline>> _compiledGetPipelinesByRepositoryAndFile
            = EF.CompileAsyncQuery((DbContext context, string repositoryId, int fileId) => context.Pipelines
                                                                                                       .Where(p => p.Repository.RepositoryId == repositoryId
                                                                                                                && p.FileId == fileId
                                                                                                                && p.Type != Pipeline.pipelineTypeRelease));

        private static readonly Func<DbContext, int, IAsyncEnumerable<int>> _compiledGetPipelineIdsByProject
            = EF.CompileAsyncQuery((DbContext context, int projectId) => context.Pipelines
                                                                                   .Where(p => p.ProjectId == projectId
                                                                                            && p.Type != Pipeline.pipelineTypeRelease)
                                                                                   .Select(p => p.PipelineId));

        private static readonly Func<DbContext, IAsyncEnumerable<DataModels.NuGetFeed>> _compiledGetNuGetFeeds
            = EF.CompileAsyncQuery((DbContext context) => context.NuGetFeeds);

        #endregion
    }
}
