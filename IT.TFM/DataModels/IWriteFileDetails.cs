using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.DataModels
{
    public interface IWriteFileDetails
    {
        void Write(FileDetails item, bool forceDetails);
    }
}
