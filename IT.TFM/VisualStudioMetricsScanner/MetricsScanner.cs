using ProjectData;
using ProjectData.Interfaces;

namespace VisualStudioMetricsScanner
{
    public class MetricsScanner : IMetricsScanner
    {
        #region IMetricsScanner Implementation

        Metrics? IMetricsScanner.Get(FileItem file)
        {
            return new Metrics();
        }

        #endregion
    }
}
