using ProjectData;
using ProjectData.Interfaces;

namespace ProjectScanner
{
    public static class ScannerFactory
    {
        public static IScanner GetScanner(string name)
        {
            var scannerType = Settings.GetSource(name);
            var instance = ServiceProviderAccessor.GetRequiredKeyedService<IScanner>(scannerType.ToString());
            instance.Initialize(name, Settings.GetConfigurationData(name));

            return instance;
        }

        public static INuGetScanner GetNuGetScanner()
        {
            var instance = ServiceProviderAccessor.GetRequiredService<INuGetScanner>();
            instance.Initialize();
            return instance;
        }

        public static IRuntimeMetricsScanner GetRuntimeMetricsScanner(IScanner scanner, string projectId, string repositoryId, string repositoryName)
        {
            var instance = ServiceProviderAccessor.GetRequiredService<IRuntimeMetricsScanner>();
            instance.Initialize(scanner, projectId, repositoryId, repositoryName);
            return instance;
        }
    }
}
