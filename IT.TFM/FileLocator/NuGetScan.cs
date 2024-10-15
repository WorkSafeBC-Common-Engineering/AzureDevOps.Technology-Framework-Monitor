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
                    var id = writer.SavePackage(package);
                    updatedPackageIds.Add(id);
                }
            }

            writer.Cleanup(updatedPackageIds.AsEnumerable());
        }

        #endregion
    }
}
