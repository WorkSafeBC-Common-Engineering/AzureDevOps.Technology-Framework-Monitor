using NuGet.Common;
using NuGet.Configuration;
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

        #endregion

        #region INuGetScanner Implementation

        void INuGetScanner.Initialize()
        {
            token = ConfidentialSettings.Values.Token;
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
            var metadata = await GetAsync(package.Feed.FeedUrl, package.Name, package.Version);
            if (metadata != null)
            {
                package.Description = metadata.Description ?? string.Empty;
                package.Authors = metadata.Authors ?? string.Empty;
                package.Published = metadata.Published.HasValue
                    ? metadata.Published.Value.DateTime
                    : null;
                package.ProjectUrl = metadata.ProjectUrl;
                package.Tags = metadata.Tags ?? string.Empty;
                package.Project = GetProject(metadata.ProjectUrl);
                package.Repository = GetRepository(metadata.ProjectUrl);
                package.Targets = metadata.DependencySets.Select(f => new NuGetTarget
                {
                    Framework = f.TargetFramework.Framework,
                    Version = f.TargetFramework.Version.ToString()
                }).ToArray();
                package.DataLoaded = true;
            }
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

            return allResults.AsEnumerable(); ;
        }

        private async Task<IPackageSearchMetadata> GetAsync(string feedUrl, string packageId, string version)
        {
            ILogger logger = NullLogger.Instance;

            var cache = new SourceCacheContext();
            var repository = GetSourceRepository(feedUrl);

            var packageVersion = new NuGetVersion(version);
            var metadataResource = await repository.GetResourceAsync<PackageMetadataResource>();

            var identity = new NuGet.Packaging.Core.PackageIdentity(packageId, packageVersion);
            var metadata = await metadataResource.GetMetadataAsync(identity, cache, logger, CancellationToken.None);

            return metadata;
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

        private static string GetRepository(Uri url)
        {
            if (url == null)
            {
                return string.Empty;
            }

            return url.Segments[1].Equals("wcbbc/", StringComparison.InvariantCultureIgnoreCase)
                ? url.Segments[4].TrimEnd('/')
                : string.Empty;
        }

        private static string GetProject(Uri url)
        {
            if (url == null)
            {
                return string.Empty;
            }

            return url.Segments[1].Equals("wcbbc/", StringComparison.InvariantCultureIgnoreCase)
                ? url.Segments[2].TrimEnd('/')
                : string.Empty;

        }

        #endregion
    }
}
