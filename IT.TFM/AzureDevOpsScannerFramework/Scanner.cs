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

        async Task<IEnumerable<Repository>> IScanner.Repositories(Project project)
        {
            var repoList = new List<Repository>();

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
                    WebUrl = r.WebUrl,
                    LastCommitId = r.LastCommitId
                };

                repoList.Add(repo);
            }

            return repoList.AsEnumerable();
        }

        async Task<IEnumerable<FileItem>> IScanner.Files(Guid projectId, Repository repository)
        {
            if (repository.DefaultBranch == null)
            {
               return null;
            }

            var fileList = new List<FileItem>();

            api.Project = projectId.ToString();
            api.Repository = repository.Id.ToString();

            var azDoFiles = await api.GetFilesAsync();

            foreach (var f in azDoFiles.Value)
            {
                if (f.IsFolder)
                {
                    continue;
                }

                var file = new FileItem
                {
                    FileType = f.Path.GetMatchedFileType(),
                    Id = f.ObjectId,
                    Path = f.Path,
                    Url = f.Url,
                    CommitId = f.CommitId
                };

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

        #endregion

        #region Private Methods

        private void ParseConfiguration(string configuration)
        {
            azureDevOpsOrganizationUrl = configuration;
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

        #endregion
    }
}
