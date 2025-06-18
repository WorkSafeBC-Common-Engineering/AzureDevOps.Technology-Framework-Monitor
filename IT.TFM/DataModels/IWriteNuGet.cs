using ProjectData;

using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IWriteNuGet
    {
        int SavePackage(NuGetPackage package);

        void Cleanup(IEnumerable<int> packageIds);
    }
}
