using AzureDevOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps
{
    public interface IRestApi
    {
        string Token { get; set; }

        string Organization { get; set; }

        string BaseUrl { get; set; }

        string Project { get; set; }

        string Repository { get; set; }

        string RepositoryBranch { get; set; }

        string CheckoutDirectory { get; set; }

        int PagingTop { get; set; }

        int PagingSkip { get; set; }

        void Initialize(string organizationUrl);

        Task<AzDoProject?> GetProjectAsync(string projectId);

        Task<AzDoProjectList> GetProjectsAsync();

        Task<AzDoRepository?> GetRepositoryAsync(string repositoryId);

        Task<AzDoRepositoryList> GetRepositoriesAsync();

        Task<AzDoFileList> GetFilesAsync();

        Task<string> DownloadRepositoryAsync();

        Task<IEnumerable<AzDoPipeline>> GetPipelinesAsync();
    }
}
