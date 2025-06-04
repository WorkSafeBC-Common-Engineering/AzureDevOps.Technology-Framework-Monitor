using Microsoft.Practices.Unity.Configuration;
using ProjectData.Interfaces;
using System.Configuration;
using Unity;

namespace ProjectScanner
{
    public static class ScannerFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "ScannerContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static ScannerFactory()
        {
            _container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        public static IScanner GetScanner(string name)
        {
            var scannerType = Settings.GetSource(name);
            var instance = _container.Resolve<IScanner>(scannerType.ToString());
            instance.Initialize(name, Settings.GetConfigurationData(name));

            return instance;
        }

        public static INuGetScanner GetNuGetScanner()
        {
            var instance = _container.Resolve<INuGetScanner>();
            instance.Initialize();
            return instance;
        }

        public static IRuntimeMetricsScanner GetRuntimeMetricsScanner()
        {
            var instance = _container.Resolve<IRuntimeMetricsScanner>();
            instance.Initialize();
            return instance;
        }
    }
}
