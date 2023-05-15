using AzureDevOps.Models;
using Newtonsoft.Json;
using RestSharp;
using System.IO.Compression;
using System.Text;

namespace AzureDevOps
{
    public class RestApi : IRestApi, IDisposable
    {
        #region Private Members

        private const string fieldBaseUrl = "{baseUrl}";
        
        private const string fieldOrganization = "{organization}";

        private const string fieldProject = "{project}";

        private const string fieldRepository = "{repository}";

        private const string fieldRepositoryBranch = "{branch}";

        private const string fieldApiVersion = "{apiVersion}";

        private const string fieldPagingTop = "{$top}";

        private const string fieldPagingSkip = "{$skip}";

        private const string apiVersion = "api-version=7.0";

        private const string getProjectsUrl = "https://{baseUrl}/{organization}_apis/projects?$top={$top}&$skip={$skip}&{apiVersion}";

        private const string getRepositoriesUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories?{apiVersion}";

        private const string getFilesUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories/{repository}/items?recursionLevel=full&{apiVersion}";

        private const string downloadRepositoryUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories/{repository}/items?recursionLevel=full&format=zip&versionDescriptor.version={branch}&versionDescriptor.versionType=branch&{apiVersion}";

        private static readonly Dictionary<string, RestClient> clients = new();

        private bool disposedValue;

        #endregion

        #region IRestApi Implementation

        public string Token { get; set; } = string.Empty;

        public string Organization { get; set; } = string.Empty;

        public string BaseUrl { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public string Repository { get; set; } = string.Empty;

        public string RepositoryBranch { get; set; } = string.Empty;

        public string CheckoutDirectory { get; set; } = string.Empty;

        public string CheckoutRepositoryDirectory
        {
            get { return Path.Combine(CheckoutDirectory, Project, Repository); }
        }

        public int PagingTop { get; set; }

        public int PagingSkip { get; set; }

        void IRestApi.Initialize(string organizationUrl)
        {
            var url = organizationUrl.EndsWith("/")
                ? organizationUrl.Substring(0, organizationUrl.Length - 1)
                : organizationUrl;

            if (url.EndsWith("visualstudio.com", StringComparison.InvariantCultureIgnoreCase))
            {
                BaseUrl = url;
                Organization = string.Empty;
                CheckoutDirectory = Path.Combine(Environment.CurrentDirectory, BaseUrl);
            }
            else
            {
                var fields = url.Split(new char[] { '/' });

                BaseUrl = fields[0];
                Organization = fields[1];
                CheckoutDirectory = Path.Combine (Environment.CurrentDirectory, Organization);
            }

            Token = Environment.GetEnvironmentVariable("TFM_AdToken");
        }

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

        async Task<AzDoFileList> IRestApi.GetFilesAsync()
        {
            var content = await CallApiAsync(GetUrl(getFilesUrl));

            var files = JsonConvert.DeserializeObject<AzDoFileList>(content);
            return files ?? new AzDoFileList();
        }

        async Task<string> IRestApi.DownloadRepositoryAsync()
        {
            return await CallApiAsync(GetUrl(downloadRepositoryUrl), mediaType:"application/zip", unzipContent:true);
        }

        #endregion

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in clients)
                    {
                        item.Value?.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RestApi()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private string GetUrl(string urlTemplate)
        {
            var orgValue = string.IsNullOrWhiteSpace(Organization)
                ? string.Empty
                : $"{Organization}/";

            return urlTemplate.Replace(fieldBaseUrl, BaseUrl)
                              .Replace(fieldApiVersion, apiVersion)
                              .Replace(fieldOrganization, orgValue)
                              .Replace(fieldProject, Project)
                              .Replace(fieldRepository, Repository)
                              .Replace(fieldRepositoryBranch, RepositoryBranch)
                              .Replace(fieldPagingTop, PagingTop.ToString())
                              .Replace(fieldPagingSkip, PagingSkip.ToString());
        }

        private string AuthHeader()
        {
            return $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{Token}"))}";
        }

        private async Task<string> CallApiAsync(string url, Method method = Method.Get, string mediaType = "application/json", bool unzipContent = false)
        {
            var restClient = GetClient(url);

            var request = new RestRequest(GetRelativeUri(url))
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
                if (Directory.Exists(CheckoutRepositoryDirectory))
                {
                    Directory.Delete(CheckoutRepositoryDirectory, true);
                }
                Directory.CreateDirectory(CheckoutRepositoryDirectory);

                var stream = new MemoryStream(response.RawBytes ?? Array.Empty<byte>());
                var archive = new ZipArchive(stream);
                archive.ExtractToDirectory(CheckoutRepositoryDirectory);
                return string.Empty;
            }

            return response.Content ?? string.Empty;
        }

        private RestClient GetClient(string url)
        {
            var uri = new Uri(url);
            var baseUrl = uri.GetLeftPart(UriPartial.Authority);

            if (clients.ContainsKey(baseUrl))
            {
                return clients[baseUrl];
            }
            else
            {
                var client = new RestClient(baseUrl);
                clients[baseUrl] = client;
                return client;
            }
        }

        private static Uri GetRelativeUri(string url)
        {
            var uri = new Uri(url);
            var baseUri = new Uri(uri.GetLeftPart(UriPartial.Authority));
            return baseUri.MakeRelativeUri(uri);
        }
        #endregion
    }
}