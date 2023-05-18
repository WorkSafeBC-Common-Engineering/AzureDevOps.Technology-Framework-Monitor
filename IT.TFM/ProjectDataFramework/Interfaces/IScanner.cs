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

        IAsyncEnumerable<Repository> Repositories(Project project);

        Task<IEnumerable<FileItem>> Files(Guid projectId, Repository repository);

        Task LoadFiles(Guid projectID, Guid repositoryId);

        void DeleteFiles();

        FileItem FileDetails(Guid projectId, Guid repositoryId, FileItem file);

        IEnumerable<string> FilePropertyNames { get; }
    }
}
