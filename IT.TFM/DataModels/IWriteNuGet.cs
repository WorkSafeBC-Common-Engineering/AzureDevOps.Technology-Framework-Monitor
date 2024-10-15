using ProjectData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.DataModels
{
    public interface IWriteNuGet
    {
        int SavePackage(NuGetPackage package);

        void Cleanup(IEnumerable<int> packageIds);
    }
}
