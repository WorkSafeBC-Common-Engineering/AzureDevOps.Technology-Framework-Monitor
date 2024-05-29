using ProjectData.Interfaces;

using RepoScan.DataModels;

using DataStorage = Storage;

namespace RepoScan.Storage.SqlServer
{
    public class FileDetails : IWriteFileDetails
    {
        #region IWriteFileDetails Implementation

        void IWriteFileDetails.Write(DataModels.FileDetails item, bool forceDetails)
        {
            using var writer = GetWriter();
            var fileItem = new ProjectData.FileItem
            {
                Id = item.Id,
                Url = item.Url,
                FileType = item.FileType,
                Path = item.Path,
                CommitId = item.CommitId
            };

            foreach (var filteredItem in item.FilteredItems)
            {
                fileItem.FilteredItems.Add(filteredItem.Key, filteredItem.Value);
            }

            foreach (var property in item.Properties)
            {
                fileItem.AddProperty(property.Key, property.Value);
            }

            foreach (var fileRef in item.References)
            {
                fileItem.AddReference(fileRef);
            }

            foreach (var pkg in item.PackageReferences)
            {
                fileItem.AddPackageReference(pkg.PackageType, pkg.Id, pkg.Version, pkg.FrameworkVersion, string.Empty);
            }

            foreach (var url in item.UrlReferences)
            {
                fileItem.AddUrlReference(url.Url, url.Path);
            }

            writer.SaveFile(fileItem, item.Repository.RepositoryId, true, forceDetails);
        }

        void IWriteFileDetails.Delete(DataModels.FileDetails item)
        {
            using var writer = GetWriter();
            var fileItem = new ProjectData.FileItem
            {
                Id = item.Id,
                Url = item.Url,
                FileType = item.FileType,
                Path = item.Path,
                CommitId = item.CommitId
            };

            writer.DeleteFile(fileItem, item.Repository.RepositoryId);
        }

        #endregion

        #region Private Methods

        private static IStorageWriter GetWriter()
        {
            return DataStorage.StorageFactory.GetStorageWriter();
        }

        #endregion
    }
}
