using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IScanner
    {
        void Initialize(string name, string configuration);

        Organization GetOrganization();

        IAsyncEnumerable<Project> Projects();

        Task<IEnumerable<Repository>> Repositories(Project project);

        Task<IEnumerable<FileItem>> Files(Guid projectId, Repository repository);

        Task LoadFiles(Guid projectId, Guid repositoryId);

        void DeleteFiles();

        FileItem FileDetails(Guid projectId, Guid repositoryId, FileItem file);

        IEnumerable<string> FilePropertyNames { get; }
    }
}
