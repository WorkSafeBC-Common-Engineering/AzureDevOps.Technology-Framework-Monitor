using ProjectData;

using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadFileItem
    {
        IEnumerable<FileItem> Read();

        IEnumerable<FileItem> Read(string repoId);

        IEnumerable<FileItem> YamlRead(string repoId);

        IEnumerable<FileItem> ReadDetails();

        IEnumerable<FileItem> ReadPropertiesForFileType(FileItemType fileType, string property = null, string value = null);
    }
}
