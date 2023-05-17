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

        private static readonly AzureDevOps.IRestApi api = new AzureDevOps.RestApi();

        #endregion

        #region IScanner Implementation

        void IScanner.Initialize(string name, string configuration)
        {
            organizationName = name;
            this.configuration = configuration;
            ParseConfiguration(this.configuration);

            api.Initialize(azureDevOpsOrganizationUrl);

            Authenticate();

            UseFileFilter();
        }

        Organization IScanner.GetOrganization()
        {
            return new Organization(ProjectSource.AzureDevOps, organizationName, azureDevOpsOrganizationUrl);
        }

        async IAsyncEnumerable<Project> IScanner.Projects()
        {
            int projectOffset = 0;
            api.PagingTop = projectCount;
            bool getMore = true;

            while (getMore)
            {
                api.PagingSkip = projectOffset;
                var azDoProjects = await api.GetProjectsAsync();

                if (azDoProjects.Value.Any())
                {
                    if (azDoProjects.Count < projectCount)
                    {
                        getMore = false;
                    }

                    foreach (var p in azDoProjects.Value)
                    {
                        var project = new Project
                        {
                            Name = p.Name,
                            Id = new Guid(p.Id),
                            Abbreviation = p.Abbreviation,
                            Description = p.Description,
                            LastUpdate = DateTime.Parse(p.LastUpdateTime),
                            Revision = long.Parse(p.Revision),
                            State = p.ProjectState,
                            Url = p.Url,
                            Visibility = p.Visibility
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

        async IAsyncEnumerable<Repository> IScanner.Repositories(Project project)
        {
            api.Project = project.Id.ToString();
            var azDoRepos = await api.GetRepositoriesAsync();

            foreach (var r in azDoRepos.Value)
            {
                var repo = new Repository
                {
                    Id = new Guid(r.Id),
                    Name = r.Name,
                    DefaultBranch = r.DefaultBranch,
                    IsFork = r.IsFork,
                    Size = r.Size,
                    Url = r.Url,
                    RemoteUrl = r.RemoteUrl,
                    WebUrl = r.WebUrl
                };

                yield return repo;
            }
        }

        async Task<IEnumerable<FileItem>> IScanner.Files(Guid projectId, Repository repository, bool loadDetails)
        {
            if (repository.DefaultBranch == null)
            {
               return null;
            }

            var fileList = new List<FileItem>();

            api.Project = projectId.ToString();
            api.Repository = repository.Id.ToString();

            var azDoFiles = await api.GetFilesAsync();
            if (loadDetails)
            {
                await api.DownloadRepositoryAsync();
            }

            foreach (var f2 in azDoFiles.Value)
            {
                if (f2.IsFolder)
                {
                    continue;
                }

                var file = new FileItem
                {
                    FileType = f2.Path.GetMatchedFileType(),
                    Id = f2.ObjectId,
                    Path = f2.Path,
                    Url = f2.Url,
                    SHA1 = f2.CommitId
                };

                if (loadDetails)
                {
                    string[] content;

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

                fileList.Add(file);
            }

            return fileList;
        }

        async Task IScanner.LoadFiles(Guid projectId, Guid repositoryId)
        {
            api.Project = projectId.ToString();
            api.Repository = repositoryId.ToString();
            await api.DownloadRepositoryAsync();
        }

        void IScanner.DeleteFiles()
        {
            if (Directory.Exists(api.CheckoutDirectory))
            {
                Directory.Delete(api.CheckoutDirectory, true);
            }
        }

        FileItem IScanner.FileDetails(Guid projectId, Guid repositoryId, FileItem file)
        {
            string[] content;
            bool hasProperties = false;

            var dir = file.Path.StartsWith("/") ? file.Path.Substring(1) : file.Path;
            var filePath = Path.Combine(api.CheckoutDirectory, dir);

            //file.SHA1 = azureFile.CommitId;

            try
            {
                content = File.ReadAllLines(filePath);

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

        async Task IScanner.Save(IStorageWriter storage)
        {
            var scanner = this as IScanner;

            var organization = scanner.GetOrganization();

            storage.SaveOrganization(organization);

            await foreach (var project in scanner.Projects())
            {
                storage.SaveProject(project);

                await foreach (var repo in scanner.Repositories(project))
                {
                    storage.SaveRepository(repo);

                    var fileList = await scanner.Files(project.Id, repo, true);

                    foreach (var file in fileList)
                    {
                        storage.SaveFile(file, repo.Id, true, false);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

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
