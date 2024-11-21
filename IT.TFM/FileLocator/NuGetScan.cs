using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class NuGetScan
    {
        #region Public Methods

        public static async Task Run()
        {
            var scanner = ScannerFactory.GetNuGetScanner();
            var reader = StorageFactory.GetNuGetReader();
            var writer = StorageFactory.GetNuGetWriter();

            var updatedPackageIds = new List<int>();
            var feeds = reader.ListFeeds();
            foreach (var feed in feeds)
            {
                var packages = await scanner.GetPackagesAsync(feed);
                foreach (var package in packages)
                {
                    await scanner.GetMetadata(package);

                    if (string.IsNullOrWhiteSpace(package.Repository))
                    {
                        GetRepositoryFromFileProperties(package);
                    }

                    var id = writer.SavePackage(package);
                    updatedPackageIds.Add(id);
                }
            }

            writer.Cleanup(updatedPackageIds.AsEnumerable());
        }

        #endregion

        #region Private Methods

        private static void GetRepositoryFromFileProperties(NuGetPackage package)
        {
            var reader = StorageFactory.GetFileItemReader();
            var items = reader.ReadPropertiesForFileType(FileItemType.Nuspec, "Id", package.Name);

            if (!items.Any())
            {
                //Cannot find a repo, leave blank
                return;
            }

            var repositories = items.Select(r => r.Repository.RepositoryId)
                                    .Distinct()
                                    .ToArray();

            if (repositories.Length > 1)
            {
                // More than one repo possible - leave blank
                return;
            }

            // have a single distinct Repository Id - use this
            package.RepositoryId = repositories[0].ToString();
        }

        #endregion
    }
}
