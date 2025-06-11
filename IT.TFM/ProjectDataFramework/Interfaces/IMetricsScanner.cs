using System.Collections.Generic;

namespace ProjectData.Interfaces
{
    public interface IMetricsScanner
    {
        Dictionary<string, Metrics> Get(FileItem file, string basePath);
    }
}
