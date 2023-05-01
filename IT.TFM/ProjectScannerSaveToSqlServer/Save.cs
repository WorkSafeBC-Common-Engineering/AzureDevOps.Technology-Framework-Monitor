using ProjData = ProjectData;
using ProjectData.Interfaces;
using ProjectScannerSaveToSqlServer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Runtime.Remoting.Messaging;
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
                    RepositoryId = repository.Id.ToString()
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

            // remove all the tildes and carrrots in spreadsheet

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

            if (nameTokens?.Length > 1 && nameTokens[0].ToUpper() != "ZZZ")
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

            var dbFile = context.Files
                                .SingleOrDefaultAsync(f => f.RepositoryId == repositoryId
                                                        && f.FileId.Equals(file.Id, StringComparison.InvariantCultureIgnoreCase)
                                                        && f.Url.Equals(file.Url, StringComparison.InvariantCultureIgnoreCase))
                                .Result;

            if (dbFile != null && !forceDetails
                && (    (dbFile.SHA1 == null && (file.SHA1 == null || !saveDetails))
                     || (dbFile.SHA1 != null && dbFile.SHA1.Equals(file.SHA1, StringComparison.InvariantCultureIgnoreCase))
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
                    FileId = file.Id
                };

                context.Files.Add(dbFile);
            }

            dbFile.FileType = context.FileTypes.SingleAsync(ft => ft.Value.Equals(file.FileType.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                               .Result;
            dbFile.Path = file.Path;
            dbFile.Url = file.Url;

            if (saveDetails)
            {
                dbFile.SHA1 = file.SHA1;

                SaveFileProperties(dbFile, file.Properties, PropertyTypeProperty);
                SaveFileProperties(dbFile, file.FilteredItems, PropertyTypeFilteredItem);

                SaveFileReferences(dbFile, file.References);
                SaveUrlReferences(dbFile, file.UrlReferences);
                SavePkgReferences(dbFile, file.PackageReferences);
            }

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
                var reference = references.SingleOrDefault(r => r.PackageType.Equals(dbRef.PackageType, StringComparison.InvariantCultureIgnoreCase)
                                                             && r.Id.Equals(dbRef.Name, StringComparison.InvariantCultureIgnoreCase));
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
                var dbReference = dbReferences.SingleOrDefault(r => r.PackageType.Equals(reference.PackageType, StringComparison.InvariantCultureIgnoreCase)
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

        private void SaveFileProperty(File file, KeyValuePair<string, string> property)
        {
            var dbProperty = file.FileProperties
                                 .SingleOrDefault(fp => fp.PropertyTypeId == PropertyTypeProperty
                                                     && fp.Name.Equals(property.Key, StringComparison.InvariantCultureIgnoreCase));

            if (dbProperty == null)
            {
                dbProperty = new FileProperty
                {
                    FileId = file.Id,
                    Name = property.Key,
                    Value = property.Value,
                    PropertyTypeId = PropertyTypeProperty
                };

                context.FileProperties.Add(dbProperty);
            }

            dbProperty.Value = property.Value;
        }

        private void SaveFileReference(File file, string reference)
        {
            var dbFileRef = file.FileReferences
                                .SingleOrDefault(fr => fr.FileReferenceTypeId == RefTypeFile
                                                    && fr.Name.Equals(reference, StringComparison.InvariantCultureIgnoreCase));

            if (dbFileRef == null)
            {
                var refType = context.FileReferenceTypes.Single(r => r.Id == RefTypeFile);

                context.FileReferences.Add(new FileReference
                {
                    FileId = file.Id,
                    FileReferenceType = refType,
                    Name = reference
                });
            }
        }

        private void SaveUrlReference(File file, ProjData.UrlReference reference)
        {
            var dbUrlRef = file.FileReferences
                                 .SingleOrDefault(ur => ur.FileReferenceTypeId == RefTypeUrl
                                                     && ur.Name.Equals(reference.Url, StringComparison.InvariantCultureIgnoreCase)
                                                     && ur.Path.Equals(reference.Path, StringComparison.InvariantCultureIgnoreCase));

            if (dbUrlRef == null)
            {
                var refType = context.FileReferenceTypes.Single(r => r.Id == RefTypeUrl);

                context.FileReferences.Add(new FileReference
                {
                    FileId = file.Id,
                    FileReferenceType = refType,
                    Name = reference.Url,
                    Path = reference.Path
                });
            }
        }

        private void SavePackageReference(File file, ProjData.PackageReference reference)
        {
            var dbPkgRef = file.FileReferences
                                 .SingleOrDefault(pr => pr.FileReferenceTypeId == RefTypePkg
                                                     && pr.PackageType.Equals(reference.PackageType, StringComparison.InvariantCultureIgnoreCase)
                                                     && pr.Name.Equals(reference.Id, StringComparison.InvariantCultureIgnoreCase));

            if (dbPkgRef == null)
            {
                var refType = context.FileReferenceTypes.Single(r => r.Id == RefTypePkg);

                dbPkgRef = new FileReference
                {
                    FileId = file.Id,
                    FileReferenceType = refType,
                    PackageType = reference.PackageType,
                    Name = reference.Id
                };

                context.FileReferences.Add(dbPkgRef);
            }

            dbPkgRef.Version = reference.Version;
            dbPkgRef.FrameworkVersion = reference.FrameworkVersion;
        }

        private void SaveFilteredItem(File file, KeyValuePair<string, string> item)
        {
            var dbItem = file.FileProperties
                             .SingleOrDefault(fi => fi.PropertyTypeId == PropertyTypeFilteredItem
                                                 && fi.Name.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase));

            if (dbItem == null)
            {
                dbItem = new FileProperty
                {
                    FileId = file.Id,
                    Name = item.Key,
                    PropertyTypeId = PropertyTypeFilteredItem
                };

                context.FileProperties.Add(dbItem);
            }

            dbItem.Value = item.Value;
        }

        private void CleanupFileProperties(ProjData.FileItem file, int dbFileId)
        {
            var dbFile = context.Files.SingleOrDefault(f => f.Id == dbFileId);
            if (dbFile == null)
            {
                return;
            }

            foreach (var property in dbFile.FileProperties)
            {
                if(!file.Properties.Any(p => p.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.FileProperties.Remove(property);
                }
            }
        }

        private void CleanupFileReferences(ProjData.FileItem file, int dbFileId)
        {
            var dbFile = context.Files.SingleOrDefault(f => f.Id == dbFileId);
            if (dbFile == null)
            {
                return;
            }

            foreach (var reference in dbFile.FileReferences)
            {
                bool found = true;

                switch (reference.FileReferenceTypeId)
                {
                    case RefTypeFile:
                        found = file.References.Any(r => r.Equals(reference.Name, StringComparison.InvariantCultureIgnoreCase));
                        break;

                    case RefTypeUrl:
                        found = file.UrlReferences.Any(ur => ur.Url.Equals(reference.Name, StringComparison.InvariantCultureIgnoreCase)
                                                          && ur.Path.Equals(reference.Path, StringComparison.InvariantCultureIgnoreCase));
                        break;

                    case RefTypePkg:
                        found = file.PackageReferences.Any(pr => pr.PackageType == reference.PackageType
                                                              && pr.Id == reference.Name);
                        break;

                    default:
                        continue;
                }

                if (!found)
                {
                    context.FileReferences.Remove(reference);
                }
            }
        }

        private void CleanupRepositoryFiles(ProjData.Repository repo, int dbRepoId)
        {
            var dbRepo = context.Repositories.SingleOrDefault(r => r.Id == dbRepoId);
            if (dbRepo == null)
            {
                return;
            }

            foreach (var file in dbRepo.Files)
            {
                if(!repo.Files.Any(f => f.Id.Equals(file.FileId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.FileProperties.RemoveRange(file.FileProperties);
                    context.FileReferences.RemoveRange(file.FileReferences);
                    context.Files.Remove(file);
                }
            }
        }

        private void CleanupProjectRepositories(ProjData.Project project, int dbProjectId)
        {
            var dbProject = context.Projects.SingleOrDefault(p => p.Id == dbProjectId);
            if (dbProject == null)
            {
                return;
            }

            foreach (var repo in dbProject.Repositories)
            {
                if (!project.Repositories.Any(r => r.Id.ToString().Equals(repo.RepositoryId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    foreach (var file in repo.Files)
                    {
                        context.FileProperties.RemoveRange(file.FileProperties);
                        context.FileReferences.RemoveRange(file.FileReferences);
                    }

                    context.Files.RemoveRange(repo.Files);
                    context.Repositories.Remove(repo);
                }
            }
        }

        private void CleanupOrganizationProjects(ProjData.Organization org, int dbOrgId)
        {
            var dbOrganization = context.Organizations.SingleOrDefault(o => o.Id == dbOrgId);
            if (dbOrganization == null)
            {
                return;
            }

            foreach (var project in dbOrganization.Projects)
            {
                if (!org.Projects.Any(p => p.Id.ToString().Equals(project.ProjectId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    foreach (var repo in project.Repositories)
                    {
                        foreach (var file in repo.Files)
                        {
                            context.FileProperties.RemoveRange(file.FileProperties);
                            context.FileReferences.RemoveRange(file.FileReferences);
                        }

                        context.Files.RemoveRange(repo.Files);
                        context.Repositories.Remove(repo);
                    }

                    context.Projects.Remove(project);
                }
            }
        }

        #endregion
    }
}
