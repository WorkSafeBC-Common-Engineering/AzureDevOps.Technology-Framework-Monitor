using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

using ProjectData;
using ProjectData.Interfaces;

namespace NuGetScanner
{
    public class Scanner : INuGetScanner
    {
        #region Private Members

        private string token = string.Empty;
        private string organization = string.Empty;

        #endregion

        #region INuGetScanner Implementation

        void INuGetScanner.Initialize()
        {
            token = ConfidentialSettings.Values.Token;
            organization = ConfidentialSettings.Values.Organization;
        }

        async Task<IEnumerable<NuGetPackage>> INuGetScanner.GetPackagesAsync(NuGetFeed feed)
        {
            var items = await ListAsync(feed.FeedUrl);
            var packages = items.Select(p => new NuGetPackage
            {
                Name = p.Identity.Id,
                Version = p.Identity.Version.OriginalVersion,
                Feed = feed
            });

            return packages;
        }

        async Task INuGetScanner.GetMetadata(NuGetPackage package)
        {
            await GetAsync(package);
        }

        #endregion

        #region Private Methods

        private async Task<IEnumerable<IPackageSearchMetadata>> ListAsync(string feedUrl)
        {
            // Create a source repository
            var repository = GetSourceRepository(feedUrl);

            // Get the PackageSearchResource
            var searchResource = await repository.GetResourceAsync<PackageSearchResource>();

            // Define the search filter
            var searchFilter = new SearchFilter(includePrerelease: true);

            // Initialize variables for pagination
            int skip = 0;
            int take = 100;
            bool hasMoreResults = true;

            var allResults = new List<IPackageSearchMetadata>();

            while (hasMoreResults)
            {
                // Search for packages with an empty string to get all packages
                var searchResults = await searchResource.SearchAsync(
                    "", // Empty string to search for all packages
                    searchFilter,
                    skip: skip,
                    take: take, // Number of packages to take
                    NullLogger.Instance,
                    CancellationToken.None);

                allResults.AddRange(searchResults);
                hasMoreResults = searchResults.Count() == take;
                skip += take;
            }

            return allResults.AsEnumerable();
        }

        private async Task GetAsync(NuGetPackage package)
        {
            ILogger logger = NullLogger.Instance;

            var cache = new SourceCacheContext();
            var repository = GetSourceRepository(package.Feed.FeedUrl);

            var packageVersion = new NuGetVersion(package.Version);
            var metadataResource = await repository.GetResourceAsync<PackageMetadataResource>();

            var identity = new NuGet.Packaging.Core.PackageIdentity(package.Name, packageVersion);

            IPackageSearchMetadata? metadata = null;
            try
                {
                // Check if the package exists in the feed
                metadata = await metadataResource.GetMetadataAsync(identity, cache, logger, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NuGetScanner GetAsync: Exception when calling GetMetadataAsync(): {ex.Message}");
            }

            if (metadata != null)
            {
                package.Description = metadata.Description ?? string.Empty;
                package.Authors = metadata.Authors ?? string.Empty;
                package.Published = metadata.Published.HasValue
                    ? metadata.Published.Value.DateTime
                    : null;
                package.Tags = metadata.Tags ?? string.Empty;
                package.Targets = [.. metadata.DependencySets.Select(f => new NuGetTarget
                {
                    Framework = f.TargetFramework.Framework,
                    Version = f.TargetFramework.Version.ToString()
                })];
                package.DataLoaded = true;
            }

            var nuspecResource = await repository.GetResourceAsync<FindPackageByIdResource>();

            using var packageStream = new MemoryStream();
            await nuspecResource.CopyNupkgToStreamAsync(package.Name, packageVersion, packageStream, cache, logger, CancellationToken.None);
            packageStream.Seek(0, SeekOrigin.Begin);

            using var packageReader = new PackageArchiveReader(packageStream);
            var nuspecReader = await packageReader.GetNuspecReaderAsync(CancellationToken.None);
            var repositoryData = nuspecReader.GetRepositoryMetadata();

            var url = repositoryData?.Url;
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            package.ProjectUrl = new Uri(url);
            package.Project = GetProject(package.ProjectUrl);
            package.Repository = GetRepository(package.ProjectUrl);
        }

        private SourceRepository GetSourceRepository(string feedUrl)
        {
            return string.IsNullOrEmpty(token)
                ? NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3(feedUrl)
                : new SourceRepository(new PackageSource(feedUrl)
                {
                    Credentials = new PackageSourceCredential(feedUrl, "PAT", token, isPasswordClearText: true, validAuthenticationTypesText: null)
                }, NuGet.Protocol.Core.Types.Repository.Provider.GetCoreV3());
        }

        private string GetRepository(Uri url)
        {
            if (url == null)
            {
                return string.Empty;
            }

            return url.Segments[1].Equals($"{organization}/", StringComparison.InvariantCultureIgnoreCase)
                ? url.Segments[4].TrimEnd('/')
                : string.Empty;
        }

        private string GetProject(Uri url)
        {
            if (url == null)
            {
                return string.Empty;
            }

            return url.Segments[1].Equals($"{organization}/", StringComparison.InvariantCultureIgnoreCase)
                ? url.Segments[2].TrimEnd('/')
                : string.Empty;

        }

        #endregion
    }
}
