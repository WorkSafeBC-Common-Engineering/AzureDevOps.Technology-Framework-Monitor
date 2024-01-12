using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.DataModels
{
    public interface IReadRepoList
    {
        IEnumerable<RepositoryItem> Read(string projectId, string repositoryId);
    }
}
