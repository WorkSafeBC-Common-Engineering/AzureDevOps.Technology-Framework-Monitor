using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
