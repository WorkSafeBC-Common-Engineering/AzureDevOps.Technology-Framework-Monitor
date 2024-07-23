using AzureDevOps.Models;

using Newtonsoft.Json;

using System.IO.Compression;
using System.Text;

namespace AzureDevOps
{
    public class RestApi : IRestApi, IDisposable
    {
        #region Private Members

        private const int maxRetries = 3;

        private const string fieldBaseUrl = "{baseUrl}";

        private const string fieldOrganization = "{organization}";

        private const string fieldProject = "{project}";

        private const string fieldRepository = "{repository}";

        private const string fieldRepositoryBranch = "{branch}";

        private const string fieldApiVersion = "{apiVersion}";

        private const string fieldPagingTop = "{$top}";

        private const string fieldPagingSkip = "{$skip}";

        private const string apiVersion = "api-version=7.0";

        private const string getProjectUrl = "https://{baseUrl}/{organization}/_apis/projects/{project}?{apiVersion}";

        private const string getProjectsUrl = "https://{baseUrl}/{organization}_apis/projects?$top={$top}&$skip={$skip}&{apiVersion}";

        private const string getRepositoryUrl = "https://{baseUrl}/{organization}/{project}/_apis/git/repositories/{repository}?{apiVersion}";

        private const string getRepositoriesUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories?{apiVersion}";

        private const string getRepositoryCommitUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories/{repository}/commits?searchCriteria.$skip=0&searchCriteria.$top=1&{apiVersion}";

        private const string getFilesUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories/{repository}/items?recursionLevel=full&{apiVersion}";

        private const string downloadRepositoryUrl = "https://{baseUrl}/{organization}{project}/_apis/git/repositories/{repository}/items?recursionLevel=full&format=zip&versionDescriptor.version={branch}&versionDescriptor.versionType=branch&{apiVersion}";

        private const string getPipelinesUrl = "https://{baseUrl}/{organization}/{project}/_apis/pipelines?{apiVersion}";

        private const string getReleasesUrl = "https://vsrm.dev.azure.com/{organization}/{project}/_apis/release/definitions?{apiVersion}";

        private static readonly Dictionary<string, HttpClient> httpClients = [];

        private static readonly Semaphore waitOnApiCall = new(1, 1);

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

        public int PagingTop { get; set; }

        public int PagingSkip { get; set; }

        private static readonly char[] separator = ['/'];

        void IRestApi.Initialize(string organizationUrl)
        {
            var workingDirectory = GetWorkingDirectory();
            if (workingDirectory == null)
            {
                var message = "IRestApi.Initialize: unable to get working directory";
                Console.WriteLine($">>> {message}");
                throw new InvalidProgramException(message);
            }

            var url = organizationUrl.EndsWith('/')
                ? organizationUrl[..^1]
                : organizationUrl;

            if (url.EndsWith("visualstudio.com", StringComparison.InvariantCultureIgnoreCase))
            {
                BaseUrl = url;
                Organization = string.Empty;
                CheckoutDirectory = Path.Combine(workingDirectory, BaseUrl);
            }
            else
            {
                var fields = url.Split(separator);

                BaseUrl = fields[0];
                Organization = fields[1];
                CheckoutDirectory = Path.Combine(System.Environment.CurrentDirectory, Organization);
            }

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> Checkout Directory = {CheckoutDirectory}, Working Directory = {workingDirectory}");
            }

            Token = ConfidentialSettings.Values.Token;
        }

        async Task<AzDoProject?> IRestApi.GetProjectAsync(string projectId)
        {
            Project = projectId;
            var content = await CallApiAsync(GetUrl(getProjectUrl));

            var project = JsonConvert.DeserializeObject<AzDoProject>(content);
            return project;
        }

        async Task<AzDoProjectList> IRestApi.GetProjectsAsync()
        {
            var content = await CallApiAsync(GetUrl(getProjectsUrl));

            var projects = JsonConvert.DeserializeObject<AzDoProjectList>(content);
            return projects ?? new AzDoProjectList();
        }

        async Task<AzDoRepository?> IRestApi.GetRepositoryAsync(string repositoryId)
        {
            Repository = repositoryId;
            var content = await CallApiAsync(GetUrl(getRepositoryUrl));

            var repository = JsonConvert.DeserializeObject<AzDoRepository>(content);
            return repository; ;
        }

        async Task<AzDoRepositoryList> IRestApi.GetRepositoriesAsync()
        {
            var content = await CallApiAsync(GetUrl(getRepositoriesUrl));
            var repositories = JsonConvert.DeserializeObject<AzDoRepositoryList>(content);

            if (repositories == null || repositories.Value == null)
            {
                return new AzDoRepositoryList();
            }

            foreach (var repo in repositories.Value)
            {
                Repository = repo.Id;
                var commitContent = await CallApiAsync(GetUrl(getRepositoryCommitUrl));
                if (commitContent == string.Empty)
                {
                    repo.LastCommitId = string.Empty;
                    continue;
                }

                var commit = JsonConvert.DeserializeObject<AzDoCommitList>(commitContent);

                if (commit == null || commit.Value == null)
                {
                    repo.LastCommitId = string.Empty;
                }
                else
                {
                    repo.LastCommitId = commit.Count == 0
                        ? string.Empty
                        : commit.Value[0].CommitId;
                }
            }

            return repositories ?? new AzDoRepositoryList();
        }

        async Task<AzDoFileList> IRestApi.GetFilesAsync()
        {
            var content = await CallApiAsync(GetUrl(getFilesUrl));

            if (content == string.Empty)
            {
                return new AzDoFileList();
            }

            var files = JsonConvert.DeserializeObject<AzDoFileList>(content);
            return files ?? new AzDoFileList();
        }

        async Task<string> IRestApi.DownloadRepositoryAsync()
        {
            return await CallApiAsync(GetUrl(downloadRepositoryUrl), mediaType: "application/zip", unzipContent: true);
        }

        async Task<IEnumerable<AzDoPipeline>> IRestApi.GetPipelinesAsync()
        {
            var content = await CallApiAsync(GetUrl(getPipelinesUrl));

            if (content == string.Empty)
            {
                return [];
            }

            var pipelines = JsonConvert.DeserializeObject<AzDoPipelineList>(content);
            if (pipelines == null || pipelines.Value == null)
            {
                return [];
            }

            var filteredPipelines = new List<AzDoPipeline>();
            foreach (var pipeline in pipelines.Value)
            {
                if (string.IsNullOrEmpty(pipeline.Url))
                {
                    continue;
                }

                var pipelineContent = await CallApiAsync(pipeline.Url);
                if (pipelineContent == string.Empty)
                {
                    continue;
                }

                AzDoPipelineDetails? pipelineDetails = null;

                try
                {
                    pipelineDetails = JsonConvert.DeserializeObject<AzDoPipelineDetails>(pipelineContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

                if (!string.IsNullOrEmpty(Repository) && pipelineDetails?.Configuration?.Repository?.Id != Repository)
                {
                    continue;
                }

                if (pipelineDetails == null)
                {
                    continue;
                }

                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"Pipeline Type: {pipelineDetails?.Configuration?.Type}");
                }

                pipeline.Details = pipelineDetails;
                filteredPipelines.Add(pipeline);
            }


            return filteredPipelines.AsEnumerable();
        }

        async Task<AzDoReleaseList> IRestApi.ListReleasesAsync()
        {
            var content = await CallApiAsync(GetUrl(getReleasesUrl));
            if (content == string.Empty)
            {
                return new AzDoReleaseList();
            }

            var releases = JsonConvert.DeserializeObject<AzDoReleaseList>(content);
            if (releases == null)
            {
                return new AzDoReleaseList();
            }

            foreach (var release in releases.Value)
            {
                if (string.IsNullOrEmpty(release.Url))
                {
                    continue;
                }

                var releaseContent = await (CallApiAsync(release.Url));
                if (releaseContent == string.Empty)
                {
                    continue;
                }

                try
                {
                    var details = JsonConvert.DeserializeObject<AzDoReleaseDetails>(releaseContent);
                    release.Details = details;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return releases;
        }

        #endregion

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in httpClients)
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

        private async Task<string> CallApiAsync(string url, string mediaType = "application/json", bool unzipContent = false)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var semResult = waitOnApiCall.WaitOne();

                if (unzipContent && Directory.Exists(CheckoutDirectory))
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($">>> CallApiAsync - unzipContent: start by deleting the checkout directory {CheckoutDirectory} first.");
                    }

                    Directory.Delete(CheckoutDirectory, true);
                }

                HttpClient httpClient = GetClient(url);

                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"API Call: {url}");
                    startTime = DateTime.Now;
                }

                HttpResponseMessage response;

                int retries = maxRetries;
                while (true)
                {
                    try
                    {
                        using var request = new HttpRequestMessage(HttpMethod.Get, url);
                        request.Headers.Add("Authorization", AuthHeader());
                        request.Headers.Add("Accept", mediaType);

                        response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        break;
                    }
                    catch (Exception ex)
                    {
                        var msg = $"\t *** Exception occured with this URL: {url}\n{ex}";
                        Console.WriteLine(msg);

                        if (retries-- <= 0)
                        {
                            throw;
                        }

                        Thread.Sleep(5000 * (maxRetries - retries));
                    }
                }

                ThrottleApi(response);

                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"End API Call, duration = {(DateTime.Now - startTime).TotalMilliseconds}");
                    startTime = DateTime.Now;
                }

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Repository is empty
                        return string.Empty;
                    }

                    throw new HttpRequestException($"API Request failed: {response.StatusCode} {response.ReasonPhrase}");
                }

                if (unzipContent)
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($">>> CallApiAsync - unzipContent: extract files to {CheckoutDirectory}");
                    }

                    GetZipContent(response.Content.ReadAsStream());

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($"Unzip, duration = {(DateTime.Now - startTime).TotalMilliseconds}");
                    }

                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                waitOnApiCall.Release();
            }
        }

        private static HttpClient GetClient(string url)
        {
            var uri = new Uri(url);
            var baseUrl = uri.GetLeftPart(UriPartial.Authority);

            if (httpClients.TryGetValue(baseUrl, out HttpClient? value))
            {
                if (value != null)
                {
                    return value;
                }
            }

            var client = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = new TimeSpan(1, 0, 0),     // one hour
            };


            httpClients[baseUrl] = client;
            return client;
        }

        private static void ThrottleApi(HttpResponseMessage response)
        {
            var headers = response.Headers;
            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                var headerName = header.Key?.ToLower();
                if (headerName == null)
                {
                    continue;
                }

                var headerValue = header.Value?.ToString();

                if (Parameters.Settings.ExtendedLogging)
                {
                    if (headerName.StartsWith("x-ratelimit") || headerName.Equals("retry-after"))
                    {
                        Console.WriteLine($"Azure API Throttling: {headerName} = {headerValue ?? "<null>"}");
                    }
                }

                if (headerValue == null)
                {
                    // invalid value, skip for now
                    continue;
                }

                if (!int.TryParse(headerValue, out int value))
                {
                    // unable to parse value - skipping
                    continue;
                }

                switch (headerName)
                {
                    case "retry-after":
                        Thread.Sleep(value * 1000);
                        break;

                    case "x-ratelimit-resource":
                        // should only be displayed, not used for computation
                        break;

                    case "x-ratelimit-delay":
                        break;

                    case "x-ratelimit-limit":
                        break;

                    case "x-ratelimit-remaining":
                        break;

                    case "x-ratelimit-reset":
                        var resetTime = DateTimeOffset.FromUnixTimeSeconds(value);
                        break;
                }
            }
        }

        private void GetZipContent(Stream zipStream)
        {
            ZipFile.ExtractToDirectory(zipStream, CheckoutDirectory, true);
        }

        private static string? GetWorkingDirectory()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location);
        }

        #endregion
    }
}