using ProjectData;
using ProjectData.Interfaces;

namespace ProjectScanner
{
    public static class MetricsScannerFactory
    {
        public static IMetricsScanner GetScanner(string name)
        {
            return ServiceProviderAccessor.GetRequiredKeyedService<IMetricsScanner>(name);
        }
    }
}
