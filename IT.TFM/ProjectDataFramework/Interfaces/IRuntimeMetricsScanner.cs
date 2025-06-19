using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IRuntimeMetricsScanner
    {
        void Initialize(IScanner scanner, string projectId, string repositoryId, string repositoryName);

        Task RunAsync();
    }
}
