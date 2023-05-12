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

        IEnumerable<FileItem> Files(Guid projectId, Repository repository, bool loadDetails);

        FileItem FileDetails(Guid repositoryId, FileItem file);

        IEnumerable<string> FilePropertyNames { get; }

        Task Save(IStorageWriter storage);
    }
}
