using AzureDevOps.Models;
using Newtonsoft.Json;
using RestSharp;
using System.IO.Compression;
using System.Text;

namespace AzureDevOps
{
    public class RestApi : IRestApi
    {
        #region Private Members

        private const string fieldBaseUrl = "{baseUrl}";
        
        private const string fieldOrganization = "{organization}";

        private const string fieldProject = "{project}";

        private const string fieldRepository = "{repository}";

        private const string fieldRepositoryBranch = "{branch}";

        private const string fieldApiVersion = "{apiVersion}";

        private const string apiVersion = "api-version=7.0";

        private const string getProjectsUrl = "https://{baseUrl}/{organization}/_apis/projects?{apiVersion}";

        private const string getRepositoriesUrl = "https://{baseUrl}/{organization}/{project}/_apis/git/repositories?{apiVersion}";

        private const string getFilesUrl = "https://{baseUrl}/{organization}/{project}/_apis/git/repositories/{repository}/items?{apiVersion}";

        private const string downloadRepositoryUrl = "https://{baseUrl}/{organization}/{project}/_apis/git/repositories/{repository}/items?recursionLevel=full&format=zip&versionDescriptor.version={branch}&versionDescriptor.versionType=branch&{apiVersion}";

        private RestClient? restClient;

        #endregion

        #region IRestApi Implementation

        public string Token { get; set; } = string.Empty;

        public string Organization { get; set; } = string.Empty;

        public string BaseUrl { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public string Repository { get; set; } = string.Empty;

        public string RepositoryBranch { get; set; } = string.Empty;

        public string CheckoutDirectory { get; set; } = string.Empty;

        async Task<AzDoProjectList> IRestApi.GetProjectsAsync()
        {
            var content = await CallApiAsync(GetUrl(getProjectsUrl));

            var projects = JsonConvert.DeserializeObject<AzDoProjectList>(content);
            return projects ?? new AzDoProjectList();
        }

        async Task<AzDoRepositoryList> IRestApi.GetRepositoriesAsync()
        {
            var content = await CallApiAsync(GetUrl(getRepositoriesUrl));

            var repositories = JsonConvert.DeserializeObject<AzDoRepositoryList>(content);
            return repositories ?? new AzDoRepositoryList();
        }

        async Task<string> IRestApi.DownloadRepositoryAsync()
        {

            return await CallApiAsync(GetUrl(downloadRepositoryUrl), mediaType:"application/zip", unzipContent:true);
        }

        #endregion

        #region Private Methods

        private string GetUrl(string urlTemplate)
        {
            return urlTemplate.Replace(fieldBaseUrl, BaseUrl)
                              .Replace(fieldApiVersion, apiVersion)
                              .Replace(fieldOrganization, Organization)
                              .Replace(fieldProject, Project)
                              .Replace(fieldRepository, Repository)
                              .Replace(fieldRepositoryBranch, RepositoryBranch);
        }

        private string AuthHeader()
        {
            return $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{Token}"))}";
        }

        private async Task<string> CallApiAsync(string url, Method method = Method.Get, string mediaType = "application/json", bool unzipContent = false)
        {
            var uri = new Uri(url);
            var baseUri = new Uri(uri.GetLeftPart(UriPartial.Authority));
            var relativeUri = baseUri.MakeRelativeUri(uri);

            if (restClient == null)
            {
                var options = new RestClientOptions(baseUri);
                restClient = new RestClient(options);
            }

            var request = new RestRequest(relativeUri)
            {
                Method = method
            };

            request.AddHeader("Authorization", AuthHeader());
            request.AddHeader("Accept", mediaType);

            var response = await restClient.ExecuteAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Request failed: {response.StatusCode} {response.ErrorMessage}");
            }

            if (unzipContent)
            {
                if (Directory.Exists(CheckoutDirectory))
                {
                    Directory.Delete(CheckoutDirectory, true);
                }
                Directory.CreateDirectory(CheckoutDirectory);

                var stream = new MemoryStream(response.RawBytes ?? Array.Empty<byte>());
                var archive = new ZipArchive(stream);
                archive.ExtractToDirectory(CheckoutDirectory);
                return string.Empty;
            }


            return response.Content ?? string.Empty;
        }

        #endregion
    }
}