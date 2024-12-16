using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IMetricsScanner
    {
        Metrics? Get(FileItem file, string basePath);
    }
}
