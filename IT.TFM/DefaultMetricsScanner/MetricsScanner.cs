using ProjectData;
using ProjectData.Interfaces;

namespace DefaultMetricsScanner
{
    public class MetricsScanner : IMetricsScanner
    {
        #region IMetricsScanner Implementation

        Dictionary<string, Metrics> IMetricsScanner.Get(FileItem file, string basePath)
        {
            return [];
        }

        #endregion
    }
}
