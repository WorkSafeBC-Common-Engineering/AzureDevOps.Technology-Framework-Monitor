using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using ProjectData;
using ProjectData.Interfaces;

namespace AzureDevOpsScannerFramework
{
    public class Scanner : IScanner
    {
        #region Private Members

        private const int projectCount = 100;

        private string configuration;
        private string azureDevOpsOrganizationUrl;

        private VssConnection connection;

        private readonly List<string> propertyFields = new List<string>();

        private string organizationName;

        private bool useFileFilter = false;

        #endregion

        #region IScanner Implementation

        void IScanner.Initialize(string name, string configuration)
        {
            organizationName = name;
            this.configuration = configuration;
            ParseConfiguration(this.configuration);

            Authenticate();

            UseFileFilter();
        }

        Organization IScanner.GetOrganization()
        {
            return new Organization(ProjectSource.AzureDevOps, organizationName, azureDevOpsOrganizationUrl);
        }

        IEnumerable<Project> IScanner.Projects()
        {
            //TODO: start of migrating to using the AzDo REST APIs
            var task = TestGetProjects();
            task.Wait();



            int projectOffset = 0;
            bool getMore = true;

            while (getMore)
            {
                var azureProjects = LoadProjects(projectCount, projectOffset);

                if (azureProjects.Any())
                {
                    if (azureProjects.Count() < projectCount)
                    {
                        getMore = false;
                    }

                    foreach (var p in azureProjects)
                    {
                        var project = new Project
                        {
                            Name = p.Name,
                            Id = p.Id,
                            Abbreviation = p.Abbreviation,
                            Description = p.Description,
                            LastUpdate = p.LastUpdateTime,
                            Revision = p.Revision,
                            State = p.State.ToString(),
                            Url = p.Url,
                            Visibility = p.Visibility.ToString()
                        };

                        yield return project;
                    }

                    projectOffset += projectCount;
                }
                else
                {
                    getMore = false;
                }
            }
        }

        IEnumerable<Repository> IScanner.Repositories(Project project)
        {
            var azureRepos = LoadRepositories(project.Id);
            foreach (var r in azureRepos)
            {
                var repo = new Repository
                {
                    Id = r.Id,
                    Name = r.Name,
                    DefaultBranch = r.DefaultBranch,
                    IsFork = r.IsFork,
                    Size = r.Size ?? -1,
                    Url = r.Url,
                    RemoteUrl = r.RemoteUrl,
                    WebUrl = r.WebUrl
                };

                yield return repo;
            }
        }

        IEnumerable<FileItem> IScanner.Files(Guid projectId, Repository repository, bool loadDetails)
        {
            if (repository.DefaultBranch == null)
            {
                yield break;
            }

            var azureFiles = LoadFiles(repository.Id);

            string[] content;

            foreach (var f in azureFiles)
            {
                var file = new FileItem
                {
                    FileType = f.Path.GetMatchedFileType(),
                    Id = f.ObjectId,
                    Path = f.Path,
                    Url = f.Url,
                    SHA1 = f.CommitId
                };

                if (loadDetails)
                {
                    try
                    {
                        content = GetFile(repository.Id, file);
                    }
                    catch (Exception ex)
                    {
                        file.AddProperty("Error", $"Unable to retrieve file. File: {file.Path}, Error Msg: {ex.Message}");
                        continue;
                    }

                    if (useFileFilter)
                    {
                        FileFiltering.Filter.FilterFile(file, content);
                    }

                    if (file.FileType != FileItemType.NoMatch)
                    {
                        AddFileProperties(file, content);
                    }

                    AddPropertyFields(file);
                }

                yield return file;
            }
        }

        FileItem IScanner.FileDetails(Guid repositoryId, FileItem file)
        {
            string[] content;
            bool hasProperties = false;

            var azureFile = LoadFile(repositoryId, file.Path);
            if (azureFile == null)
            {
                return null;
            }

            file.SHA1 = azureFile.CommitId;

            try
            {
                content = GetFile(repositoryId, file);

                if (useFileFilter)
                {
                    FileFiltering.Filter.FilterFile(file, content);
                }

                if (file.FileType != FileItemType.NoMatch)
                {
                    AddFileProperties(file, content);
                }

                AddPropertyFields(file);
                hasProperties = file.Properties.Any() || file.References.Any() || file.UrlReferences.Any() || file.PackageReferences.Any();

            }
            catch (Exception ex)
            {
                if (hasProperties)
                {
                    file.AddProperty("Error", $"Unable to retrieve file. File: {file.Path}, Error Msg: {ex.Message}");
                }
            }

            if (hasProperties)
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        IEnumerable<string> IScanner.FilePropertyNames => propertyFields.AsEnumerable();

        void IScanner.Save(IStorageWriter storage)
        {
            var scanner = this as IScanner;

            var organization = scanner.GetOrganization();

            storage.SaveOrganization(organization);

            foreach (var project in scanner.Projects())
            {
                storage.SaveProject(project);

                foreach (var repo in scanner.Repositories(project))
                {
                    storage.SaveRepository(repo);

                    foreach (var file in scanner.Files(project.Id, repo, true))
                    {
                        storage.SaveFile(file, repo.Id, true, false);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task TestGetProjects()
        {
            AzureDevOps.IRestApi api = new AzureDevOps.RestApi
            {
                BaseUrl = "nvisionideas.visualstudio.com",
                Organization = "",
                Token = Environment.GetEnvironmentVariable("TFM_AdToken")
            };

            var projects = await api.GetProjectsAsync();

            api.Project = projects.Value[0].Id;
            var repos = await api.GetRepositoriesAsync();

            api.Repository = repos.Value[0].Id;

            var branchPath = repos.Value[0].DefaultBranch;  // refs/heads/master
            var branchFields = branchPath.Split('/');
            var branch = branchFields[branchFields.Length - 1];

            api.RepositoryBranch = branch;
            api.CheckoutDirectory = @"C:\x\AzureDevOps.Technology-Framework-Monitor\CheckoutDir";
            await api.DownloadRepositoryAsync();
        }


        private void ParseConfiguration(string configuration)
        {
            azureDevOpsOrganizationUrl = configuration;
        }

        private void Authenticate()
        {
            try
            {
                connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), GetVssCredentials());
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
        }

        private IEnumerable<TeamProjectReference> LoadProjects(int amount, int offset)
        {
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
            IEnumerable<TeamProjectReference> projects = projectClient.GetProjects(null, amount, offset, null, null, null).Result;

            return projects;
        }

        public VssCredentials GetVssCredentials()
        {
            var mgr = GetCredentialsManager();
            return mgr.Get();
        }

        private IEnumerable<GitRepository> LoadRepositories(Guid projectId)
        {
            var client = connection.GetClient<GitHttpClient>();
            return client.GetRepositoriesAsync(projectId).Result.AsEnumerable();
        }

        private IEnumerable<GitItem> LoadFiles(Guid repoId)
        {
            try
            {
                var client = connection.GetClient<GitHttpClient>();
                return client.GetItemsAsync(repoId, null, VersionControlRecursionType.Full).Result.AsEnumerable();
            }
            catch (AggregateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving files from {repoId}: {ex}");
                return new List<GitItem>().AsEnumerable();
            }
        }

        private GitItem LoadFile(Guid repoId, string filePath)
        {
            GitItem item = null;
            var retries = 2;

            while (retries > 0)
            {
                try
                {
                    var client = connection.GetClient<GitHttpClient>();
                    item = client.GetItemAsync(repoId, filePath).Result;
                    break;
                }
                catch (AggregateException ex)
                {
                    var skipFile = false;

                    foreach (var innerEx in ex.InnerExceptions)
                    {
                        if (innerEx is VssServiceException vssException && vssException.Message.StartsWith("TF401174:"))
                        {
                            // File not found in repo - skip this file
                            skipFile = true;
                            break;
                        }
                    }

                    if (skipFile)
                    {
                        retries = 0;
                    }
                    else
                    {
                        retries--;
                        Authenticate();
                    }
                }
            }

            return item;
        }

        private Stream GetFileStream(Guid repoId, FileItem item)
        {
            var client = connection.GetClient<GitHttpClient>();
            return client.GetItemTextAsync(repoId, item.Path, null, null, null, null, null, null, true).Result;
        }

        private void AddFileProperties(FileItem file, string[] content)
        {
            try
            {
                Parser.ParseFile.GetProperties(file, content);
            }
            catch (Exception ex)
            {
                file.AddProperty("Error", $"Unable to parse file. File: {file.Path}, Error Msg: {ex.Message}");
                return;
            }
        }

        private string[] GetFile(Guid repositoryId, FileItem item)
        {
            var stream = GetFileStream(repositoryId, item);
            using (var reader = new StreamReader(stream))
            {
                var content = new List<string>();
                while (!reader.EndOfStream)
                {
                    content.Add(reader.ReadLineAsync().Result);
                }

                return content.ToArray();
            }
        }

        private void AddPropertyFields(FileItem file)
        {
            var newFields = file.Properties
                                .Select(f => f.Key)
                                .Where(k => !propertyFields.Contains(k));

            propertyFields.AddRange(newFields);
        }

        private void UseFileFilter()
        {
            var useFilterValue = ConfigurationManager.AppSettings["useFileFiltering"];
            bool.TryParse(useFilterValue, out useFileFilter);
        }

        private ScannerCommon.Interfaces.ICredentials GetCredentialsManager()
        {
            return CredentialsFactory.GetCredentialsManager();
        }

        #endregion
    }
}
