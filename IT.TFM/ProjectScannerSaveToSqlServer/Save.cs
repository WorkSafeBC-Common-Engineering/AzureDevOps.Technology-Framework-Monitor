using ProjData = ProjectData;
using ProjectData.Interfaces;
using ProjectScannerSaveToSqlServer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectData;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ProjectScannerSaveToSqlServer
{
    public class Save : DbCore, IStorageWriter
    {
        #region IStorage Implementation

        void IStorageWriter.Initialize(string configuration)
        {
            Initialize(configuration);
        }

        int IStorageWriter.SaveOrganization(ProjData.Organization organization)
        {
            var org = context.Organizations
                             .SingleOrDefault(o => o.ScannerType.Value.Equals(organization.Source.ToString())
                                                && o.Uri.Equals(organization.Url));

            if (org == null)
            {
                org = new DataModels.Organization()
                {
                    Uri = organization.Url,
                    ScannerType = context.ScannerTypes
                                         .Single(t => t.Value.Equals(organization.Source.ToString()))
                };

                context.Organizations.Add(org);
            }

            org.Name = organization.Name;

            _ = context.SaveChangesAsync().Result;

            organizationId = org.Id;
            projectId = 0;
            repositoryId = 0;

            return org.Id;
        }

        int IStorageWriter.SaveProject(ProjData.Project project, int organizationId)
        {
            if (organizationId == 0)
            {
                throw new InvalidOperationException("No Organization defined");
            }

            var dbProject = context.Projects
                                   .SingleOrDefault(p => p.OrganizationId == organizationId
                                                      && p.ProjectId.Equals(project.Id.ToString()));

            if (dbProject == null)
            {
                dbProject = new DataModels.Project()
                {
                    OrganizationId = organizationId,
                    ProjectId = project.Id.ToString()
                };

                context.Projects.Add(dbProject);
            }

            dbProject.Url = project.Url;
            dbProject.Abbreviation = project.Abbreviation;
            dbProject.Description = project.Description;
            dbProject.LastUpdate = project.LastUpdate;
            dbProject.Name = project.Name;
            dbProject.Revision = project.Revision;
            dbProject.State = project.State;
            dbProject.Visibility = project.Visibility;
            dbProject.Deleted = project.Deleted;

            _ = context.SaveChangesAsync().Result;

            projectId = dbProject.Id;
            repositoryId = 0;
            return dbProject.Id;
        }

        int IStorageWriter.SaveRepository(ProjData.Repository repository, int projectId)
        {
            if (projectId == 0)
            {
                throw new InvalidOperationException("No Project defined");
            }

            var dbRepo = context.Repositories
                                .SingleOrDefault(r => r.ProjectId == projectId
                                                   && r.RepositoryId.Equals(repository.Id.ToString()));

            if (dbRepo == null)
            {
                dbRepo = new DataModels.Repository
                {
                    ProjectId = projectId,
                    RepositoryId = repository.Id.ToString(),
                    LastCommitId = string.Empty
                };

                context.Repositories.Add(dbRepo);
            }

            dbRepo.DefaultBranch = repository.DefaultBranch;
            dbRepo.IsFork = repository.IsFork;
            dbRepo.Name = repository.Name;
            dbRepo.RemoteUrl = repository.RemoteUrl;
            dbRepo.Size = repository.Size;
            dbRepo.Url = repository.Url;
            dbRepo.WebUrl = repository.WebUrl;
            dbRepo.Deleted = repository.Deleted;
            dbRepo.LastCommitId = repository.LastCommitId;

            // remove all the tildes and carets in spreadsheet

            // This code was added here as there are a few different calls into here and this processing should most likely be done for all of them.
            // Most of the repos are named as such "[Portfolio Name]-[Project Name]-[Component Name]", The projects with a first token of "zzz" are obsolete so skip if found
            // This is only looking for items with a length > 2 as we need a minimum of 2 pieces of information to fill in the fields from the repo name

            // split first token into portfolio, second token application name, third token component.,
            string[] nameTokens = repository.Name.Split('-');

            // its likely we have a case where the seperation token was a '.' instead
            if (nameTokens.Length < 2)
            {
                nameTokens = repository.Name.Split('.');
            }

            if (nameTokens?.Length > 1 && !nameTokens[0].Equals("ZZZ", StringComparison.CurrentCultureIgnoreCase))
            {
                dbRepo.Portfolio = nameTokens[0];
                
                
                dbRepo.ApplicationProjectName = nameTokens[1];

                if (nameTokens.Length >2)
                {
                    // the first 2 elements are skipped and the rest are re-joined to create the project name, this makes sure no delimiter is in the front of the name
                    dbRepo.ComponentName = string.Join("-", nameTokens.Skip(2));
                }                    
            }
            
            _ = context.SaveChangesAsync().Result;

            repositoryId = dbRepo.Id;
            return dbRepo.Id;
        }

        void IStorageWriter.SaveFile(FileItem file, Guid repoId, bool saveDetails, bool forceDetails)
        {
            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"StorageWriter.SaveFile: writing file {file.Path}, saveDetails = {saveDetails}, forceDetails = {forceDetails}");
            }

            var id = repoId.ToString("D").ToLower();
            var repo = context.Repositories
                              .SingleOrDefaultAsync(r => r.RepositoryId == id)
                              .Result;
            int repositoryId = repo == null ? 0 : repo.Id;

            context.Database.SetCommandTimeout(3600);
            var dbFile = context.Files
                                .SingleOrDefault(f => f.RepositoryId == repositoryId
                                                   && f.Path.Equals(file.Path));

            if (dbFile != null && !forceDetails
                && (    (dbFile.CommitId == null && (file.CommitId == null || !saveDetails))
                     || (dbFile.CommitId != null && dbFile.CommitId.Equals(file.CommitId))
                     ))
            {
                // file has not been updated since last scan
                return;
            }

            if (dbFile != null && !saveDetails && !forceDetails
                && dbFile.FileType.Value.Equals(file.FileType.ToString())
                && dbFile.Path.Equals(file.Path))
            {
                return;
            }

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"StorageWriter.SaveFile: writing file {file.Path} - file will be processed");
            }


            if (dbFile == null)
            {
                dbFile = new File
                {
                    RepositoryId = repositoryId,
                    Path = file.Path
                };

                context.Files.Add(dbFile);

                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"StorageWriter.SaveFile: writing file {file.Path} - adding file");
                }
            }

            dbFile.FileType = context.FileTypes.SingleAsync(ft => ft.Value.Equals(file.FileType.ToString()))
                                               .Result;
            dbFile.FileId = file.Id;
            dbFile.Url = file.Url;
            dbFile.CommitId = file.CommitId;

            if (saveDetails)
            {
                SaveFileProperties(dbFile, file.Properties, PropertyTypeProperty);
                SaveFileProperties(dbFile, file.FilteredItems, PropertyTypeFilteredItem);

                SaveFileReferences(dbFile, file.References);
                SaveUrlReferences(dbFile, file.UrlReferences);
                SavePkgReferences(dbFile, file.PackageReferences);
            }

            _ = context.SaveChangesAsync().Result;
        }

        void IStorageWriter.SavePipeline(ProjData.Pipeline pipeline)
        {
            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"SavePipeline: PipelineId = {pipeline.Id}, ProjectId = {pipeline.ProjectId}, Repo ID = {pipeline.RepositoryId}, Pipeline Name = {pipeline.Name}, Pipeline URL = {pipeline.Url}, Pipeline Path = {pipeline.Path}");
            }

            var dbProject = context.Projects
                                   .SingleOrDefault(p => p.ProjectId == pipeline.ProjectId);

            var dbRepo = context.Repositories
                                .SingleOrDefault(r => r.RepositoryId.Equals(pipeline.RepositoryId));

            var dbPipeline = context.Pipelines
                                    .SingleOrDefault(p => p.PipelineId == pipeline.Id && p.Type != ProjData.Pipeline.pipelineTypeRelease);

            var dbFile = dbRepo != null ? context.Files
                                                 .SingleOrDefault(f => f.RepositoryId == dbRepo.Id
                                                                    && f.Path.Equals(pipeline.Path))
                                        : null;

            if (dbPipeline == null)
            {
                dbPipeline = new DataModels.Pipeline
                {
                    PipelineId = pipeline.Id,
                };

                context.Pipelines.Add(dbPipeline);
            }

            dbPipeline.ProjectId = dbProject?.Id;
            dbPipeline.RepositoryId = dbRepo?.Id;
            dbPipeline.Name = pipeline.Name;
            dbPipeline.Folder = pipeline.Folder;
            dbPipeline.Revision = pipeline.Revision;
            dbPipeline.Url = pipeline.Url;
            dbPipeline.Type = pipeline.Type;
            dbPipeline.PipelineType = pipeline.PipelineType;
            dbPipeline.Path = pipeline.Path;
            dbPipeline.FileId = dbFile?.Id;
            dbPipeline.State = pipeline.State;
            dbPipeline.Result = pipeline.Result;
            dbPipeline.LastRunUrl = pipeline.LastRunUrl;
            dbPipeline.LastRunStart = pipeline.LastRunStart;
            dbPipeline.LastRunEnd = pipeline.LastRunEnd;

            // For Classic pipelines we can immediately parse for the Portfolio and Product based on the JSON details.
            // For Yaml pipelines, this happens at a later stage when we parse the actual file, so leave as is in the database.
            if (pipeline.Type == ProjData.Pipeline.pipelineTypeClassic)
            {
                dbPipeline.Portfolio = pipeline.Portfolio;
                dbPipeline.Product = pipeline.Product;
            }

            else if (pipeline.Type == ProjData.Pipeline.pipelineTypeYaml &&
                (!string.IsNullOrEmpty(pipeline.YamlType) ||
                 !string.IsNullOrEmpty(pipeline.Portfolio) ||
                 !string.IsNullOrEmpty(pipeline.Product)))
            {
                dbPipeline.YamlType = pipeline.YamlType;
                dbPipeline.Portfolio = pipeline.Portfolio;
                dbPipeline.Product = pipeline.Product;
            }

            _ = context.SaveChangesAsync().Result;
        }

        void IStorageWriter.SaveRelease(Release release)
        {
            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"SavePipeline: PipelineId = {release.Id}, ProjectId = {release.ProjectId}, Pipeline Name = {release.Name}, Pipeline URL = {release.Url}, Pipeline Path = {release.Folder}");
            }

            var dbProject = context.Projects
                                   .SingleOrDefault(p => p.ProjectId == release.ProjectId);

            var dbPipeline = context.Pipelines
                                    .SingleOrDefault(p => p.ProjectId == dbProject.Id
                                                       && p.PipelineId == release.Id
                                                       && p.Type == ProjData.Pipeline.pipelineTypeRelease);

            if (dbPipeline == null)
            {
                dbPipeline = new DataModels.Pipeline
                {
                    ProjectId = dbProject?.Id,
                    PipelineId = release.Id
                };

                context.Pipelines.Add(dbPipeline);
            }

            dbPipeline.Name = release.Name;
            dbPipeline.Folder = release.Folder;
            dbPipeline.Revision = release.Revision;
            dbPipeline.Url = release.Url;
            dbPipeline.Type = release.Type;
            dbPipeline.PipelineType = release.PipelineType;

            dbPipeline.Source = release.Source;
            dbPipeline.CreatedById = release.CreatedById;
            dbPipeline.CreatedByName = release.CreatedByName;
            dbPipeline.CreatedDateTime = release.CreatedDateTime;
            dbPipeline.ModifiedById = release.ModifiedById;
            dbPipeline.ModifiedByName = release.ModifiedByName;
            dbPipeline.ModifiedDateTime = release.ModifiedDateTime;
            dbPipeline.IsDeleted = release.IsDeleted;
            dbPipeline.IsDisabled = release.IsDisabled;
            dbPipeline.LastReleaseId = release.LastReleaseId;
            dbPipeline.LastReleaseName = release.LastReleaseName;
            dbPipeline.LastRunStart = release.LastRunStart;
            dbPipeline.LastRunEnd = release.LastRunEnd;
            dbPipeline.LastRunUrl = release.LastRunUrl;
            dbPipeline.State = release.State;
            dbPipeline.Result = release.Result;
            dbPipeline.Environments = string.Join('|', release.Environments);

            _ = context.SaveChangesAsync().Result;

            var currentArtifacts = context.ReleaseArtifacts.Where(a => a.PipelineId == dbPipeline.Id);
            foreach (var artifact in release.Artifacts)
            {
                var dbArtifact = currentArtifacts.SingleOrDefault(a => a.SourceId.Equals(artifact.SourceId)
                                                                    && a.Alias.Equals(artifact.Alias));

                if (dbArtifact == null)
                {
                    dbArtifact = new()
                    {
                        PipelineId = dbPipeline.Id,
                        SourceId = artifact.SourceId,
                        Alias = artifact.Alias
                    };
                    context.ReleaseArtifacts.Add(dbArtifact);
                }

                dbArtifact.Url = artifact.Url;
                dbArtifact.Type = artifact.Type;
                dbArtifact.DefaultVersionType = artifact.DefaultVersionType;
                dbArtifact.DefinitionId = artifact.DefinitionId;
                dbArtifact.DefinitionName = artifact.DefinitionName;
                dbArtifact.Project = artifact.Project;
                dbArtifact.ProjectId = artifact.ProjectId;
                dbArtifact.IsPrimary = artifact.IsPrimary;
                dbArtifact.IsRetained = artifact.IsRetained;
            }

            _ = context.SaveChangesAsync().Result;
        }

        void IStorageWriter.LinkPipelineToFile(int pipelineId, string repositoryId, string path)
        {
            var pipeline = context.Pipelines.SingleOrDefault(p => p.Id == pipelineId
                                                               && p.Repository.RepositoryId == repositoryId
                                                               && p.Type != ProjData.Pipeline.pipelineTypeRelease);
            if (pipeline != null)
            {
                var file = context.Files.SingleOrDefault(f => f.Repository.RepositoryId == repositoryId
                                                           && f.Path.Equals(pipeline.Path));
                
                pipeline.FileId = file?.Id;

                _ = context.SaveChangesAsync().Result;
            }
        }

        void IStorageWriter.DeleteFile(FileItem file, Guid repoId)
        {
            var id = repoId.ToString("D").ToLower();
            var repo = context.Repositories
                              .SingleOrDefaultAsync(r => r.RepositoryId == id)
                              .Result;
            int repositoryId = repo == null ? 0 : repo.Id;

            context.Database.SetCommandTimeout(3600);
            var dbFile = context.Files
                                .SingleOrDefault(f => f.RepositoryId == repositoryId
                                                   && f.Path.Equals(file.Path));
            if (dbFile == null)
            {
                return;
            }

            if (dbFile.FileReferences.Count > 0)
            {
                var fileRefList = dbFile.FileReferences.ToArray();
                foreach (var fileRef in fileRefList)
                {
                    context.FileReferences.Remove(fileRef);
                }
            }

            if (dbFile.FileProperties.Count > 0)
            {
                var filePropList = dbFile.FileProperties.ToArray();
                foreach (var fileProp in filePropList)
                {
                    context.FileProperties.Remove(fileProp);
                }
            }

            var pipelines = context.Pipelines.Where(p => p.FileId == dbFile.Id);
            foreach(var pipeline in pipelines)
            {
                Debug.WriteLine($"Deleting File {dbFile.Id}, pipeline = {pipeline.PipelineId}");
                pipeline.FileId = null;
            }

            _ = context.SaveChangesAsync().Result;

            context.Files.Remove(dbFile);
            _ = context.SaveChangesAsync().Result;
        }

        void IStorageWriter.DeletePipeline(int pipelineId)
        {
            var pipeline = context.Pipelines.SingleOrDefault(p => p.PipelineId == pipelineId
                                                               && p.Type != ProjData.Pipeline.pipelineTypeRelease);
            if (pipeline != null)
            {
                context.Entry(pipeline).State = EntityState.Deleted;
                _ = context.SaveChangesAsync().Result;
            }
        }

        int IStorageWriter.SaveNuGetPackage(ProjData.NuGetPackage package)
        {
            var dbFeed = context.NuGetFeeds
                                .SingleOrDefault(f => f.FeedUrl.Equals(package.Feed.FeedUrl)) ?? throw new Exception("Feed not found");

            var dbRepository = package.RepositoryId != null
                ? context.Repositories
                         .SingleOrDefault(r => r.RepositoryId == package.RepositoryId)
                : context.Repositories
                         .SingleOrDefault(r => r.Name == package.Repository
                                            && r.Project.Name == package.Project);

            var dbPackage = context.NuGetPackages
                                   .SingleOrDefault(p => p.Name.Equals(package.Name));

            if (dbPackage == null)
            {
                dbPackage = new()
                {
                    Name = package.Name
                };

                context.NuGetPackages.Add(dbPackage);
            }

            dbPackage.Version = package.Version;
            dbPackage.Description = package.Description;
            dbPackage.Authors = package.Authors;
            dbPackage.Published = package.Published ?? DateTime.MinValue;
            dbPackage.ProjectUrl = package.ProjectUrl?.ToString();
            dbPackage.Tags = package.Tags;
            dbPackage.NuGetFeedId = dbFeed.Id;
            dbPackage.RepositoryId = dbRepository?.Id;

            _ = context.SaveChangesAsync().Result;

            foreach (var target in package.Targets)
            {
                var dbTarget = context.NuGetTargetFrameworks
                                      .SingleOrDefault(t => t.Framework.Equals(target.Framework)
                                                         && t.NuGetPackageId == dbPackage.Id);

                if (dbTarget == null)
                {
                    dbTarget = new()
                    {
                        Framework = target.Framework,
                        NuGetPackageId = dbPackage.Id
                    };

                    context.NuGetTargetFrameworks.Add(dbTarget);
                }
                
                dbTarget.Version = target.Version;
            }

            var deletedTargets = context.NuGetTargetFrameworks
                                        .Where(t => t.NuGetPackageId == dbPackage.Id)
                                        .AsEnumerable();

            if (deletedTargets != null)
            {
                foreach (var target in deletedTargets)
                {
                    if (!package.Targets.Any(pt => pt.Framework.Equals(target.Framework)))
                    {
                        context.NuGetTargetFrameworks.Remove(target);
                    }
                }
            }

            _ = context.SaveChangesAsync().Result;

            return dbPackage.Id;
        }

        void IStorageWriter.CleanupNuGetPackages(IEnumerable<int> packageIds)
        {
            var packages = context.NuGetPackages
                                 .Where(p => !packageIds.Contains(p.Id))
                                 .AsEnumerable();

            foreach (var package in packages)
            {
                foreach (var target in package.NuGetTargetFrameworks)
                {
                    context.NuGetTargetFrameworks.Remove(target);
                }

                context.NuGetPackages.Remove(package);

                _ = context.SaveChangesAsync().Result;
            }
        }

        void IStorageWriter.SaveMetrics(FileItem file, Metrics metrics)
        {
            var dbRepo = context.Repositories
                    .SingleOrDefault(r => r.RepositoryId.Equals(file.RepositoryId.ToString("D")));

            var dbFile = context.Files
                                .SingleOrDefault(f => f.RepositoryId == dbRepo.Id
                                                   && f.Path.Equals(file.Path));

            var dbMetrics = dbFile.ProjectMetrics;
            if (dbMetrics == null)
            {
                dbMetrics = new ProjectMetrics
                {
                    FileId = dbFile.Id
                };

                dbFile.ProjectMetrics = dbMetrics;
            }

            dbMetrics.MaintainabilityIndex = metrics.MaintainabilityIndex;
            dbMetrics.CyclomaticComplexity = metrics.CyclomaticComplexity;
            dbMetrics.DepthOfInheritance = metrics.DepthOfInheritance;
            dbMetrics.SourceLines = metrics.SourceLines;
            dbMetrics.ExecutableLines = metrics.ExecutableLines;

            _ = context.SaveChangesAsync().Result;
        }

        #endregion

        #region Private Methods

        private void SaveFileProperties(File dbFile, Dictionary<string, string> properties, int propertyType)
        {
            var propertyNames = properties.Select(p => p.Key).ToList();
            var deletedDbProperties = new List<FileProperty>();

            var dbProperties = dbFile.FileProperties.Where(p => p.PropertyTypeId == propertyType).ToList();

            // First remove any database properties that are not in the new list
            foreach (var dbProperty in dbProperties)
            {
                if (!propertyNames.Contains(dbProperty.Name))
                {
                    context.FileProperties.Remove(dbProperty);
                    deletedDbProperties.Add(dbProperty);
                }
            }

            foreach (var deleted in deletedDbProperties)
            {
                dbProperties.Remove(deleted);
            }

            deletedDbProperties.Clear();

            // Next update any of the existing database properties that are in the list
            foreach (var dbProperty in dbProperties)
            {
                var property = properties.SingleOrDefault(p => p.Key.Equals(dbProperty.Name));
                if (property.Key != null)
                {
                    dbProperty.Value = property.Value;
                }
            }

            // Finally add any new properties from the list that are not in the database for this file
            foreach (var property in properties)
            {
                var dbProperty = dbProperties.SingleOrDefault(p => p.Name.Equals(property.Key));

                if (dbProperty == null)
                {
                    dbFile.FileProperties.Add(new FileProperty
                    {
                        FileId = dbFile.Id,
                        PropertyTypeId = propertyType,
                        Name = property.Key,
                        Value = property.Value,
                    });
                }
            }
        }

        private void SaveFileReferences(File dbFile, IEnumerable<string> references)
        {
            var deletedDbReferences = new List<FileReference>();
            var dbReferences = dbFile.FileReferences.Where(r => r.FileReferenceTypeId == RefTypeFile).ToList();

            // First remove any database properties that are not in the new list
            foreach (var dbReference in dbReferences)
            {
                if (!references.Contains(dbReference.Name))
                {
                    context.FileReferences.Remove(dbReference);
                    deletedDbReferences.Add(dbReference);
                }
            }

            foreach (var deleted in deletedDbReferences)
            {
                dbReferences.Remove(deleted);
            }

            deletedDbReferences.Clear();

            // Next add any new properties from the list that are not in the database for this file
            foreach (var reference in references)
            {
                var dbReference = dbReferences.FirstOrDefault(p => p.Name.Equals(reference));

                if (dbReference == null)
                {
                    dbFile.FileReferences.Add(new FileReference
                    {
                        FileId = dbFile.Id,
                        FileReferenceTypeId = RefTypeFile,
                        Name = reference
                    });
                }
            }
        }

        private void SaveUrlReferences(File dbFile, List<ProjData.UrlReference> references)
        {
            var dbReferences = dbFile.FileReferences.Where(r => r.FileReferenceTypeId == RefTypeUrl).ToList();
            var deletedDbReferences = new List<FileReference>();

            // First remove any database properties that are not in the new list
            foreach (var dbRef in dbReferences)
            {
                if (!references.Any(r => r.Url.Equals(dbRef.Name)
                                      && r.Path.Equals(dbRef.Path)))
                {
                    context.FileReferences.Remove(dbRef);
                    deletedDbReferences.Add(dbRef);
                }
            }

            foreach (var deleted in deletedDbReferences)
            {
                dbReferences.Remove(deleted);
            }

            deletedDbReferences.Clear();

            // Next add any new properties from the list that are not in the database for this file
            foreach (var reference in references)
            {
                var dbReference = dbReferences.SingleOrDefault(p => p.Name.Equals(reference));

                if (dbReference == null)
                {
                    dbFile.FileReferences.Add(new FileReference
                    {
                        FileId = dbFile.Id,
                        FileReferenceTypeId = RefTypeUrl,
                        Name = reference.Url,
                        Path = reference.Path
                    });
                }
            }
        }

        private void SavePkgReferences(File dbFile, IEnumerable<ProjData.PackageReference> references)
        {
            var deletedDbReferences = new List<FileReference>();
            var dbReferences = dbFile.FileReferences.Where(r => r.FileReferenceTypeId == RefTypePkg).ToList();

            // First remove any database properties that are not in the new list
            foreach (var dbReference in dbReferences)
            {
                if (!references.Any(r => r.PackageType.Equals(dbReference.PackageType)
                                      && r.Id.Equals(dbReference.Name)))
                {
                    context.FileReferences.Remove(dbReference);
                    deletedDbReferences.Add(dbReference);
                }
            }

            foreach (var deleted in deletedDbReferences)
            {
                dbReferences.Remove(deleted);
            }

            deletedDbReferences.Clear();

            // Next update any of the existing database properties that are in the list
            foreach (var dbRef in dbReferences)
            {
                var reference = references.Where(r => r.PackageType.Equals(dbRef.PackageType)
                                                   && r.Id.Equals(dbRef.Name))
                                          .OrderBy(r => r.Version).Last();

                if (reference != null)
                {
                    dbRef.Version = reference.Version;
                    dbRef.VersionComparator = reference.VersionComparator;
                    dbRef.FrameworkVersion = reference.FrameworkVersion;
                }
            }

            // Finally add any new properties from the list that are not in the database for this file
            foreach (var reference in references)
            {
                var dbReference = dbReferences.FirstOrDefault(r => r.PackageType.Equals(reference.PackageType)
                                                                && r.Name.Equals(reference.Id));

                if (dbReference == null)
                {
                    dbFile.FileReferences.Add(new FileReference
                    {
                        FileId = dbFile.Id,
                        FileReferenceTypeId = RefTypePkg,
                        Name = reference.Id,
                        PackageType = reference.PackageType,
                        Version = reference.Version,
                        VersionComparator = reference.VersionComparator,
                        FrameworkVersion = reference.FrameworkVersion
                    });
                }
            }
        }

        #endregion
    }
}
