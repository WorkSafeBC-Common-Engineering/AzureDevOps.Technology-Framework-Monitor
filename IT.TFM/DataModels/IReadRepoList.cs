using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadRepoList
    {
        IEnumerable<RepositoryItem> Read(string projectId, string repositoryId);

        IEnumerable<string> GetRepositoryIds();
    }
}
