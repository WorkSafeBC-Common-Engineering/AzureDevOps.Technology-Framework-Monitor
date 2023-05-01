using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.DataModels
{
    public interface IWriteFileItem
    {
        void Write(FileItem item, bool saveDetails, bool forceDetails);
    }
}
