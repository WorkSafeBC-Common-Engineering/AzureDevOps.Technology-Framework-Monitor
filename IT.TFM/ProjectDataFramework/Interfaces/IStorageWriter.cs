using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IStorageWriter
    {
        void Initialize(string configuration);

        void SaveOrganization(Organization organization);

        void SaveProject(Project project);

        void SaveRepository(Repository repository);

        void SaveFile(FileItem file, Guid repoId, bool saveDetails, bool forceDetails);

        void DeleteFile(FileItem file, Guid repoId);

        void Close();
    }
}
