using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AzureDevOps.Models;

using Microsoft.VisualStudio.Services.Common;

using ProjectData;
using ProjectData.Interfaces;

using Artifact = ProjectData.Artifact;
using Project = ProjectData.Project;
using Repository = ProjectData.Repository;

namespace AzureDevOpsScannerFramework
{
    public class Scanner : IScanner
    {
        #region Private Members

        private const int projectCount = 100;

        private string azureDevOpsOrganizationUrl;

        private readonly List<string> propertyFields = [];

        private string organizationName;

        private bool useFileFilter = false;

        private static readonly AzureDevOps.IRestApi api = new AzureDevOps.RestApi();

        private static readonly Dictionary<string, string> buildProperties = [];

        #endregion

        #region IScanner Implementation

        void IScanner.Initialize(string name, string configuration)
        {
            organizationName = name;
            ParseConfiguration(configuration);

            api.Initialize(azureDevOpsOrganizationUrl);

            UseFileFilter();

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($">>> IScanner.Initialize: org = {organizationName}, orgUrl = {azureDevOpsOrganizationUrl}, useFileFilter = {useFileFilter}");
            }
        }

        Organization IScanner.GetOrganization()
        {
            return new Organization(ProjectSource.AzureDevOps, organizationName, azureDevOpsOrganizationUrl);
        }

        async IAsyncEnumerable<Project> IScanner.Projects(string projectId)
        {
            if (projectId != string.Empty)
            {
                var project = await api.GetProjectAsync(projectId);

                yield return new Project
                {
                    Name = project.Name,
                    Id = new Guid(project.Id),
                    Abbreviation = project.Abbreviation,
                    Description = project.Description,
                    LastUpdate = DateTime.Parse(project.LastUpdateTime),
                    Revision = long.Parse(project.Revision),
                    State = project.ProjectState,
                    Url = project.Url,
                    Visibility = project.Visibility
                };
            }

            else
            {
                int projectOffset = 0;
                api.PagingTop = projectCount;
                bool getMore = true;

                while (getMore)
                {
                    api.PagingSkip = projectOffset;
                    var azDoProjects = await api.GetProjectsAsync();

                    if (azDoProjects.Value.Length != 0)
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
        }

        async Task<IEnumerable<Repository>> IScanner.Repositories(Project project, string repositoryId)
        {
            var repoList = new List<Repository>();

            api.Project = project.Id.ToString();

            AzDoRepositoryList azDoRepos;

            if (repositoryId != string.Empty)
            {
                var repository = await api.GetRepositoryAsync(repositoryId);
                azDoRepos = new AzDoRepositoryList { Count = 1, Value = [repository] };
            }
            else
            {
                azDoRepos = await api.GetRepositoriesAsync();
            }

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

        async Task<IEnumerable<ProjectData.Pipeline>> IScanner.Pipelines(Guid projectId, string repositoryId)
        {
            var pipelineList = new List<ProjectData.Pipeline>();
            api.Project = projectId.ToString();
            api.Repository = repositoryId;
            var azDoPipelines = await api.GetPipelinesAsync();

            foreach (var pipeline in azDoPipelines)
            {
                var p = GetPipeline(pipeline);
                p.ProjectId = api.Project;

                api.Pipeline = p.Id;
                var runs = await api.GetPipelineRunsAsync();
                if (runs != null && runs.Count > 0 && runs.Value.Length > 0)
                {
                    var run = runs.Value
                                  .OrderByDescending(r => r.CreatedDate)
                                  .FirstOrDefault();

                    if (run != null)
                    {
                        p.LastRunStart = run.CreatedDate;
                        p.LastRunEnd = run.FinishedDate;
                        p.State = run.State;
                        p.Result = run.Result;
                        p.LastRunUrl = run.Url;
                    }
                }

                pipelineList.Add(p);
            }

            return pipelineList.AsEnumerable();
        }

        async Task<IEnumerable<ProjectData.Pipeline>> IScanner.Releases(Guid projectId, string repositoryId)
        {
            var pipelineList = new List<ProjectData.Pipeline>();
            api.Project = projectId.ToString();
            api.Repository = repositoryId;
            var azdoReleases = await api.ListReleasesAsync();

            foreach (var release in azdoReleases.Value)
            {
                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"Release: {release.Id} - {release.Name}");
                }

                var p = GetPipeline(release);
                p.ProjectId = api.Project;

                pipelineList.Add(p);
            }

            return pipelineList.AsEnumerable();
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
                    CommitId = f.CommitId,
                    RepositoryId = repository.Id
                };

                fileList.Add(file);
            }

            return fileList;
        }

        async Task IScanner.LoadFiles(Guid projectId, Guid repositoryId, string branch)
        {
            api.Project = projectId.ToString();
            api.Repository = repositoryId.ToString();
            api.RepositoryBranch = string.Empty;

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($">>> IScanner.LoadFiles: Project ID = {api.Project}, Repo ID = {api.Repository}, Branch = {api.RepositoryBranch}, Checkout Dir = {api.CheckoutDirectory}");
            }

            await api.DownloadRepositoryAsync();

            if (Directory.Exists(api.CheckoutDirectory))
            {
                LoadDirectoryBuildProperties();
            }
            else
            {
                Console.WriteLine($"\t >>> Project: {api.Project}, Repository: {api.Repository} - Download directory not found: {api.CheckoutDirectory}");
            }
        }

        void IScanner.DeleteFiles()
        {
            if (Directory.Exists(api.CheckoutDirectory))
            {
                DeleteDirectory(api.CheckoutDirectory);
            }
        }

        FileItem IScanner.FileDetails(Guid projectId, Guid repositoryId, FileItem file)
        {
            if (file.FileType == FileItemType.Dll)
            {
                return file;
            }

            string[] content;
            bool hasProperties = false;

            var dir = file.Path.StartsWith('/') ? file.Path[1..] : file.Path;
            var filePath = Path.Combine(api.CheckoutDirectory, dir);

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($">>> IScanner.FileDetails: processing file {filePath}");
            }

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
                hasProperties = file.Properties.Count != 0 || file.References.Count != 0 || file.UrlReferences.Count != 0 || file.PackageReferences.Count != 0 || file.PipelineProperties.Count != 0;

            }
            catch (Exception ex)
            {
                if (hasProperties)
                {
                    file.AddProperty("Error", $"Unable to retrieve file. File: {file.Path}, Error Msg: {ex.Message}");
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($">>> IScanner.FileDetails: Unable to retrieve file. File: {{file.Path}}, Error Msg: {{ex.Message}}");
                    }
                }
            }

            if (hasProperties)
            {
                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($">>> IScanner.FileDetails: found properties = {hasProperties}");
                }

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

        private static void AddFileProperties(FileItem file, string[] content)
        {
            try
            {
                Parser.ParseFile.GetProperties(file, content, buildProperties);
            }
            catch (Exception ex)
            {
                file.AddProperty("Error", $"Unable to parse file. File: {file.Path}, Error Msg: {ex.Message}");
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
            if (!bool.TryParse(useFilterValue, out useFileFilter))
            {
                useFileFilter = false;
            }
        }

        private static void LoadDirectoryBuildProperties()
        {
            buildProperties.Clear();

            Directory.EnumerateFiles(api.CheckoutDirectory, "Directory.Build.props", SearchOption.AllDirectories)
                .ForEach(f =>
                {
                    var xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.Load(f);

                    var propertyGroups = xmlDocument.GetElementsByTagName("PropertyGroup");
                    for (int i = 0; i < propertyGroups.Count; i++)
                    {
                        var propertyGroup = propertyGroups[i];
                        for (int j = 0; j < propertyGroup.ChildNodes.Count; j++)
                        {
                            var property = propertyGroup.ChildNodes[j];
                            if (property.NodeType == System.Xml.XmlNodeType.Element)
                            {
                                var key = property.Name;
                                var value = property.InnerText;

                                buildProperties.TryAdd(key, value);
                            }
                        }
                    }
                });
        }

        private static ProjectData.Pipeline GetPipeline(AzDoPipeline pipeline)
        {
            var p = new ProjectData.Pipeline
            {
                Id = pipeline.Id ?? 0,
                Name = pipeline.Name,
                Folder = pipeline.Folder,
                Revision = pipeline.Revision ?? 0,
                Url = pipeline.Url,
                Type = pipeline.Details.Configuration.Type,
            };

            Console.WriteLine($"Pipeline: {pipeline.Name}, Type: {pipeline.Details.Configuration.Type}");

            switch (pipeline.Details.Configuration.Type)
            {
                case "designerJson":
                    p.PipelineType = pipeline.Details.Configuration.DesignerJson.Type;
                    p.RepositoryId = pipeline.Details.Configuration.DesignerJson.Repository.Id;

                    var portfolioProduct = pipeline.Details.Configuration?.DesignerJson?.Variables?.PortfolioProductName?.Value;
                    if (portfolioProduct != null)
                    {
                        var index = portfolioProduct.IndexOf('.');
                        if (index > 0)
                        {
                            p.Portfolio = portfolioProduct[..index];
                            p.Product = portfolioProduct[(index + 1)..];
                        }
                    }

                    break;

                case "designerHyphenJson":
                    break;

                case "justInTime":
                    break;

                case "unknown":
                    break;

                case "yaml":
                    p.RepositoryId = pipeline.Details.Configuration.Repository.Id;
                    p.Path = pipeline.Details.Configuration.Path;
                    break;
            }

            return p;
        }

        private static ProjectData.Release GetPipeline(AzDoRelease release)
        {
            var pipeline = new ProjectData.Release
            {
                Id = release.Id,
                Name = release.Name,
                Folder = release.Path,
                Url = release.Url,
                Type = ProjectData.Pipeline.pipelineTypeRelease,
                PipelineType = ProjectData.Pipeline.pipelineRelease,
                Revision = release.Revision,
                Source = release.Source,
                CreatedByName = release.CreatedBy.DisplayName,
                CreatedById = release.CreatedBy.UniqueName,
                CreatedDateTime = release.CreatedOn,
                ModifiedByName = release.ModifiedBy?.DisplayName,
                ModifiedById = release.ModifiedBy?.UniqueName,
                ModifiedDateTime = release.ModifiedOn,
                IsDeleted = release.IsDeleted,
                IsDisabled = release.IsDisabled,
                LastReleaseId = release.Details?.LastRelease?.Id ?? 0,
                LastReleaseName = release.Details?.LastRelease?.Name,
                LastRunStart = release.LastRunStart,
                LastRunEnd = release.LastRunEnd,
                LastRunUrl = release.LastRunUrl,
                State = release.State,
                Result = release.Result,
                Environments = release.Details.Environments.Select(e => e.Name).ToArray(),
                Artifacts = release.Details.Artifacts.Select(a => new Artifact
                    {
                        SourceId = a.SourceId,
                        Type = a.Type,
                        Alias = a.Alias,
                        Url = a.DefinitionReference?.ArtifactSourceDefinitionUrl?.Id,
                        DefaultVersionType = a.DefinitionReference?.DefaultVersionType?.Name,
                        DefinitionId = a.DefinitionReference?.Definition?.Id,
                        DefinitionName = a.DefinitionReference?.Definition?.Name,
                        Project = a.DefinitionReference?.Project?.Name,
                        ProjectId = a.DefinitionReference?.Project?.Id,
                        IsPrimary = a.IsPrimary ?? false,
                        IsRetained = a.IsRetained ?? false
                    }).ToArray()
            };

            return pipeline;
        }

        /// <summary>
        /// Delete a directory and all its contents
        /// </summary>
        /// <remarks>
        /// Directory.Delete() will fail on files that are flagged as read-only. I found this method on StackOverflow
        /// (https://stackoverflow.com/questions/1157246/unauthorizedaccessexception-trying-to-delete-a-file-in-a-folder-where-i-can-dele)
        /// that can overcome this issue.
        /// </remarks>
        /// <param name="targetDir">directory to be deleted</param>
        private static void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        #endregion
    }
}
