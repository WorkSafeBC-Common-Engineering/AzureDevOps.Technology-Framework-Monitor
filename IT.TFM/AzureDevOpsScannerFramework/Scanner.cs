﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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

        async Task<IEnumerable<Pipeline>> IScanner.Pipelines(Guid projectId, string repositoryId)
        {
            var pipelineList = new List<Pipeline>();
            api.Project = projectId.ToString();
            api.Repository = repositoryId;
            var azDoPipelines = await api.GetPipelinesAsync();

            foreach (var pipeline in azDoPipelines)
            {
                var p = GetPipeline(pipeline);
                p.ProjectId = api.Project;
                pipelineList.Add(p);
            }

            return pipelineList.AsEnumerable();
        }

        async Task<IEnumerable<Pipeline>> IScanner.Releases(Guid projectId, string repositoryId)
        {
            var pipelineList = new List<Pipeline>();
            api.Project = projectId.ToString();
            api.Repository = repositoryId;
            var azdoReleases = await api.ListReleasesAsync();
            foreach (var release in azdoReleases.value)
            {
#if DEBUG
                Console.WriteLine($"Release: {release.id} - {release.name}");
#endif
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

        async Task IScanner.LoadFiles(Guid projectId, Guid repositoryId)
        {
            api.Project = projectId.ToString();
            api.Repository = repositoryId.ToString();
            await api.DownloadRepositoryAsync();
            LoadDirectoryBuildProperties();
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

            var dir = file.Path.StartsWith('/') ? file.Path[1..] : file.Path;
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
                hasProperties = file.Properties.Count != 0 || file.References.Count != 0 || file.UrlReferences.Count != 0 || file.PackageReferences.Count != 0 || file.PipelineProperties.Count != 0;

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

        private static Pipeline GetPipeline(AzDoPipeline pipeline)
        {
            var p = new Pipeline
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

        private static Release GetPipeline(AzDoRelease release)
        {
            var pipeline = new Release
            {
                Id = release.id,
                Name = release.name,
                Folder = release.path,
                Url = release.url,
                Type = Pipeline.pipelineTypeRelease,
                PipelineType = Pipeline.pipelineRelease,
                Revision = release.revision,
                Source = release.source,
                CreatedByName = release.createdBy.displayName,
                CreatedById = release.createdBy.uniqueName,
                CreatedDateTime = release.createdOn,
                ModifiedByName = release.modifiedBy?.displayName,
                ModifiedById = release.modifiedBy?.uniqueName,
                ModifiedDateTime = release.modifiedOn,
                IsDeleted = release.isDeleted,
                IsDisabled = release.isDisabled,
                LastReleaseId = release.Details?.lastRelease?.id ?? 0,
                LastReleaseName = release.Details?.lastRelease?.name,
                Environments = release.Details.environments.Select(e => e.name).ToArray(),
                Artifacts = release.Details.artifacts.Select(a => new Artifact
                    {
                        SourceId = a.sourceId,
                        Type = a.type,
                        Alias = a.alias,
                        Url = a.definitionReference?.artifactSourceDefinitionUrl?.id,
                        DefaultVersionType = a.definitionReference?.defaultVersionType?.name,
                        DefinitionId = a.definitionReference?.definition?.id,
                        DefinitionName = a.definitionReference?.definition?.name,
                        Project = a.definitionReference?.project?.name,
                        ProjectId = a.definitionReference?.project?.id,
                        IsPrimary = a.isPrimary,
                        IsRetained = a.isRetained
                    }).ToArray()
            };

            return pipeline;
        }

        #endregion
    }
}
