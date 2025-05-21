using ProjectData.Interfaces;
using RepoScan.DataModels;
using DataStorage = Storage;
using System.Collections.Generic;
using System.Linq;
using ProjectData;

namespace RepoScan.Storage.SqlServer
{
    public class FileItem : IReadFileItem, IWriteFileItem
    {
        #region IReadFileItem Implementation

        IEnumerable<DataModels.FileItem> IReadFileItem.Read()
        {
            using var reader = GetReader();
            foreach (var item in reader.GetFiles())
            {
                var fileItem = new DataModels.FileItem
                {
                    Id = item.Id,
                    FileType = item.FileType,
                    Path = item.Path,
                    Url = item.Url
                };

                yield return fileItem;
            }
        }

        IEnumerable<DataModels.FileItem> IReadFileItem.Read(string repoId)
        {
            using var reader = GetReader();

            var repoFiles = reader.GetFiles(repoId)
                                  .ToArray();

            return repoFiles.Select(f => new DataModels.FileItem
            {
                Id = f.Id,
                FileType = f.FileType,
                Path = f.Path,
                Url = f.Url,
                CommitId = f.CommitId
            });
        }

        IEnumerable<DataModels.FileItem> IReadFileItem.ReadDetails()
        {
            using var reader = GetReader();
            foreach (var item in reader.GetFiles())
            {
                var repo = reader.GetRepository(item.RepositoryId);
                if (repo == null)
                {
                    continue;
                }

                var fileItem = new DataModels.FileItem
                {
                    Id = item.Id,
                    FileType = item.FileType,
                    Path = item.Path,
                    Url = item.Url,
                    Repository = new RepositoryItem
                    {
                        OrgName = repo.OrgName,
                        RepositoryId = repo.Id
                    }
                };

                yield return fileItem;
            }
        }

        IEnumerable<DataModels.FileItem> IReadFileItem.YamlRead(string repoId)
        {
            using var reader = GetReader();

            var repoFiles = reader.GetYamlFiles(repoId)
                                  .ToArray();

            return repoFiles.Select(f => new DataModels.FileItem
            {
                Id = f.Id,
                FileType = f.FileType,
                Path = f.Path,
                Url = f.Url,
                CommitId = f.CommitId
            });
        }

        IEnumerable<DataModels.FileItem> IReadFileItem.ReadPropertiesForFileType(FileItemType fileType, string property, string value)
        {
            var reader = GetReader();
            var files = reader.GetFilesWithProperties(fileType, property, value);

            var dataFiles = files.Select(f => new DataModels.FileItem
            {
                Id = f.Id,
                FileType = fileType,
                Path = f.Path,
                Url = f.Url,
                Repository = new RepositoryItem
                {
                    RepositoryId = f.RepositoryId
                },                
                CommitId = f.CommitId
            });

            return dataFiles;           
        }

        #endregion

        #region IWriteFileItem Implementation

        void IWriteFileItem.Write(DataModels.FileItem item, bool saveDetails, bool forceDetails)
        {
            using var writer = GetWriter();

            var fileItem = new ProjectData.FileItem
            {
                Id = item.Id,
                FileType = item.FileType,
                Path = item.Path,
                Url = item.Url,
                CommitId = item.CommitId
            };

            writer.SaveFile(fileItem, item.Repository.RepositoryId, saveDetails, forceDetails);
        }

        void IWriteFileItem.Delete(DataModels.FileItem item)
        {
            using var writer = GetWriter();

            var fileItem = new ProjectData.FileItem
            {
                Id = item.Id,
            };

            writer.DeleteFile(fileItem, item.Repository.RepositoryId);
        }

        #endregion

        #region Private Methods

        private static IStorageWriter GetWriter()
        {
            return DataStorage.StorageFactory.GetStorageWriter();
        }

        private static IStorageReader GetReader()
        {
            return DataStorage.StorageFactory.GetStorageReader();
        }

        #endregion
    }
}
