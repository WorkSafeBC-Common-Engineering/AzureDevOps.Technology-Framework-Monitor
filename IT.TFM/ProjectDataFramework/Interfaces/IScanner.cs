using System;
using System.Collections.Generic;

namespace ProjectData.Interfaces
{
    public interface IScanner
    {
        void Initialize(string name, string configuration);

        Organization GetOrganization();

        IEnumerable<Project> Projects();

        IEnumerable<Repository> Repositories(Project project);

        IEnumerable<FileItem> Files(Guid projectId, Repository repository, bool loadDetails);

        FileItem FileDetails(Guid repositoryId, FileItem file);

        IEnumerable<string> FilePropertyNames { get; }

        void Save(IStorageWriter storage);
    }
}
