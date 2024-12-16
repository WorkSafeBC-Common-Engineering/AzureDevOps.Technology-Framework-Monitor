using ProjectData;
using ProjectData.Interfaces;

namespace DefaultMetricsScanner
{
    public class MetricsScanner : IMetricsScanner
    {
        #region IMetricsScanner Implementation
        
        Metrics? IMetricsScanner.Get(FileItem file)
        {
            return null;
        }

        #endregion
    }
}
