using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface INuGetScanner
    {
        void Initialize();

        Task<IEnumerable<NuGetPackage>> GetPackagesAsync(NuGetFeed feed);

        Task GetMetadata(NuGetPackage package);
    }
}
