using ProjData = ProjectData;
using ProjectData.Interfaces;
using ProjectScannerSaveToSqlServer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ProjectScannerSaveToSqlServer
{
    public class Save : DbCore, IStorageWriter
    {
        #region IStorage Implementation

        void IStorageWriter.Initialize(string configuration)
        {
            Initialize(configuration);
        }

        void IStorageWriter.Close()
        {
            Close();
        }

        void IStorageWriter.SaveOrganization(ProjData.Organization organization)
        {
            var org = context.Organizations
                             .SingleOrDefault(o => o.ScannerType.Value.Equals(organization.Source.ToString(), StringComparison.InvariantCultureIgnoreCase)
                                                && o.Uri.Equals(organization.Url, StringComparison.InvariantCultureIgnoreCase));

            if (org == null)
            {
                org = new Organization()
                {
                    Uri = organization.Url,
                    ScannerType = context.ScannerTypes
                                         .Single(t => t.Value.Equals(organization.Source.ToString(), StringComparison.InvariantCultureIgnoreCase))
                };

                context.Organizations.Add(org);
            }

            org.Name = organization.Name;

            // CleanupOrganizationProjects(organization, org.Id);

            _ = context.SaveChangesAsync().Result;

            organizationId = org.Id;
            projectId = 0;
            repositoryId = 0;
        }

        void IStorageWriter.SaveProject(ProjData.Project project)
        {
            if (organizationId == 0)
            {
                throw new InvalidOperationException("No Organization defined");
            }

            var dbProject = context.Projects
                                   .SingleOrDefault(p => p.OrganizationId == organizationId
                                                      && p.ProjectId.Equals(project.Id.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (dbProject == null)
            {
                dbProject = new Project()
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

            // CleanupProjectRepositories(project, dbProject.Id);

            _ = context.SaveChangesAsync().Result;

            projectId = dbProject.Id;
            repositoryId = 0;
        }

        void IStorageWriter.SaveRepository(ProjData.Repository repository)
        {
            if (projectId == 0)
            {
                throw new InvalidOperationException("No Project defined");
            }

            var dbRepo = context.Repositories
                                .SingleOrDefault(r => r.ProjectId == projectId
                                                   && r.RepositoryId.Equals(repository.Id.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (dbRepo == null)
            {
                dbRepo = new Repository
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
            
            // CleanupRepositoryFiles(repository, dbRepo.Id);

            _ = context.SaveChangesAsync().Result;

            repositoryId = dbRepo.Id;
        }

        void IStorageWriter.SaveFile(ProjData.FileItem file, Guid repoId, bool saveDetails, bool forceDetails)
        {
            var id = repoId.ToString("D").ToLower();
            var repo = context.Repositories
                              .SingleOrDefaultAsync(r => r.RepositoryId == id)
                              .Result;
            int repositoryId = repo == null ? 0 : repo.Id;

            context.Database.CommandTimeout = 3600;
            var allFiles = context.Files
                                .Where(f => f.RepositoryId == repositoryId
                                                        && f.FileId.Equals(file.Id, StringComparison.InvariantCultureIgnoreCase)
                                                        && f.Url.Equals(file.Url, StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();

            var dbFile = allFiles.SingleOrDefault(f => f.Url.Equals(file.Url, StringComparison.InvariantCultureIgnoreCase));

            if (dbFile != null && !forceDetails
                && (    (dbFile.CommitId == null && (file.CommitId == null || !saveDetails))
                     || (dbFile.CommitId != null && dbFile.CommitId.Equals(file.CommitId, StringComparison.InvariantCultureIgnoreCase))
                     ))
            {
                // file has not been updated since last scan
                return;
            }

            if (dbFile != null && !saveDetails && !forceDetails
                && dbFile.FileType.Value.Equals(file.FileType.ToString(), StringComparison.InvariantCultureIgnoreCase)
                && dbFile.Path.Equals(file.Path, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (dbFile == null)
            {
                dbFile = new File
                {
                    RepositoryId = repositoryId,
                    FileId = file.Id,
                    CommitId = string.Empty
                };

                context.Files.Add(dbFile);
            }

            dbFile.FileType = context.FileTypes.SingleAsync(ft => ft.Value.Equals(file.FileType.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                               .Result;
            dbFile.Path = file.Path;
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
            var dbRepo = context.Repositories
                                .SingleOrDefault(r => r.RepositoryId.Equals(pipeline.RepositoryId, StringComparison.InvariantCultureIgnoreCase))
                                ?? throw new InvalidOperationException("No matching Repository defined");

            var dbPipeline = context.Pipelines
                                    .SingleOrDefault(p => p.PipelineId == pipeline.Id);

            if (dbPipeline == null)
            {
                dbPipeline = new Pipeline
                {
                    Repository = dbRepo,
                    PipelineId = pipeline.Id,
                    RepositoryId = dbRepo.Id
                };

                context.Pipelines.Add(dbPipeline);
            }

            dbPipeline.Name = pipeline.Name;
            dbPipeline.Folder = pipeline.Folder;
            dbPipeline.Revision = pipeline.Revision;
            dbPipeline.Url = pipeline.Url;
            dbPipeline.Type = pipeline.Type;
            dbPipeline.PipelineType = pipeline.PipelineType;
            dbPipeline.QueueStatus = pipeline.QueueStatus;
            dbPipeline.Quality = pipeline.Quality;
            dbPipeline.CreatedBy = pipeline.CreatedBy;
            dbPipeline.CreatedDate = pipeline.CreatedDate;

            _ = context.SaveChangesAsync().Result;
        }

        void IStorageWriter.DeleteFile(ProjData.FileItem file, Guid repoId)
        {
            var id = repoId.ToString("D").ToLower();
            var repo = context.Repositories
                              .SingleOrDefaultAsync(r => r.RepositoryId == id)
                              .Result;
            int repositoryId = repo == null ? 0 : repo.Id;

            context.Database.CommandTimeout = 3600;
            var allFiles = context.Files
                                .Where(f => f.RepositoryId == repositoryId
                                                        && f.FileId.Equals(file.Id, StringComparison.InvariantCultureIgnoreCase)
                                                        && f.Url.Equals(file.Url, StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();

            var dbFile = allFiles.SingleOrDefault(f => f.Url.Equals(file.Url, StringComparison.InvariantCultureIgnoreCase));

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

            context.Files.Remove(dbFile);

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
                if (!references.Any(r => r.Url.Equals(dbRef.Name, StringComparison.InvariantCultureIgnoreCase)
                                      && r.Path.Equals(dbRef.Path, StringComparison.InvariantCultureIgnoreCase)))
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
                if (!references.Any(r => r.PackageType.Equals(dbReference.PackageType, StringComparison.InvariantCultureIgnoreCase)
                                      && r.Id.Equals(dbReference.Name, StringComparison.InvariantCultureIgnoreCase)))
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
                var reference = references.Where(r => r.PackageType.Equals(dbRef.PackageType, StringComparison.InvariantCultureIgnoreCase)
                                                   && r.Id.Equals(dbRef.Name, StringComparison.InvariantCultureIgnoreCase))
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
                var dbReference = dbReferences.FirstOrDefault(r => r.PackageType.Equals(reference.PackageType, StringComparison.InvariantCultureIgnoreCase)
                                                                && r.Name.Equals(reference.Id, StringComparison.InvariantCultureIgnoreCase));

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
