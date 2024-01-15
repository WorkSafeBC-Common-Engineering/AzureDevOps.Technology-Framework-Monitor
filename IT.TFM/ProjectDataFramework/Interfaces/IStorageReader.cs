using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IStorageReader
    {
        void Initialize(string configuration);

        bool IsDatabase { get; }

        Organization GetOrganization(string projectId, string repositoryId);

        Repository GetRepository();

        Repository GetRepository(Guid id);

        IEnumerable<FileItem> GetFiles();

        IEnumerable<FileItem> GetFiles(string id);

        void Close();
    }
}
