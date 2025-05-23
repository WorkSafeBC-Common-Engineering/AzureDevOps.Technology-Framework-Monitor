using System;
using System.Collections.Generic;

namespace ProjectData.Interfaces
{
    public interface IStorageReader : IDisposable
    {
        void Initialize(string configuration);

        bool IsDatabase { get; }

        Organization GetOrganization(string projectId, string repositoryId);

        Repository GetRepository();

        Repository GetRepository(Guid id);

        IEnumerable<string> GetRepositoryIds();

        IEnumerable<FileItem> GetFiles();

        IEnumerable<FileItem> GetFiles(string id);

        IEnumerable<FileItem> GetYamlFiles(string id);

        IEnumerable<Pipeline> GetPipelines(string repositoryId, string filePath);

        IEnumerable<Pipeline> GetPipelines(string pipelineType);

        IEnumerable<Pipeline> FindPipelines(string projectId, string repositoryId, string portfolio, string product);

        IEnumerable<int> GetPipelineIdsForProject(string projectId);

        IEnumerable<NuGetFeed> GetNuGetFeeds();

        IEnumerable<FileItem> GetFilesWithProperties(FileItemType fileType, string propertyId, string propertyValue);
    }
}
