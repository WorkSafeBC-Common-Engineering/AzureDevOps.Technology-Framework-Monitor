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

        Task<AzDoProjectList> GetProjectsAsync();

        Task<AzDoRepositoryList> GetRepositoriesAsync();

        Task<string> DownloadRepositoryAsync();
    }
}
