using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadFileItem
    {
        IEnumerable<FileItem> Read();

        IEnumerable<FileItem> Read(string repoId);

        IEnumerable<FileItem> YamlRead(string repoId);

        IEnumerable<FileItem> ReadDetails();
    }
}
