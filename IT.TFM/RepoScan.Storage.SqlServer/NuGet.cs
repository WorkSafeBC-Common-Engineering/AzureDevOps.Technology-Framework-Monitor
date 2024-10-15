using ProjectData;
using ProjectData.Interfaces;

using RepoScan.DataModels;

using DataStorage = Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace RepoScan.Storage.SqlServer
{
    internal class NuGet : IReadNuGet, IWriteNuGet
    {
        #region IReadNuGet Implementation

        IEnumerable<NuGetFeed> IReadNuGet.ListFeeds()
        {
            using var reader = GetReader();
            var feeds = reader.GetNuGetFeeds().ToArray();

            return feeds.AsEnumerable();
        }

        #endregion

        #region IWriteNuGet Implementation

        int IWriteNuGet.SavePackage(NuGetPackage package)
        {
            using var writer = DataStorage.StorageFactory.GetStorageWriter();
            return writer.SaveNuGetPackage(package);
        }

        void IWriteNuGet.Cleanup(IEnumerable<int> packageIds)
        {
            using var writer = DataStorage.StorageFactory.GetStorageWriter();
            writer.CleanupNuGetPackages(packageIds);
        }

        #endregion

        #region Private Methods

        private static IStorageReader GetReader()
        {
            return DataStorage.StorageFactory.GetStorageReader();
        }

        #endregion
    }
}
