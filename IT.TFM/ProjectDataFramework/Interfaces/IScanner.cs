using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IScanner
    {
        void Initialize(string name, string configuration);

        Organization GetOrganization();

        IAsyncEnumerable<Project> Projects(string projectId);

        Task<IEnumerable<Repository>> Repositories(Project project, string repositoryId);

        Task<IEnumerable<FileItem>> Files(Guid projectId, Repository repository);

        Task LoadFiles(Guid projectId, Guid repositoryId);

        Task<IEnumerable<Pipeline>> Pipelines(Guid projectId, string repositoryId);

        Task<IEnumerable<Pipeline>> Releases(Guid projectId, string repositoryId);

        void DeleteFiles();

        FileItem FileDetails(Guid projectId, Guid repositoryId, FileItem file);

        IEnumerable<string> FilePropertyNames { get; }
    }
}
