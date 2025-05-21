using Microsoft.Practices.Unity.Configuration;

using ProjectData.Interfaces;

using System.Configuration;

using Unity;

namespace ProjectScanner
{
    public static class MetricsScannerFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "MetricsScannerContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static MetricsScannerFactory()
        {
            _container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        public static IMetricsScanner GetScanner(string name)
        {
            var instance = _container.Resolve<IMetricsScanner>(name);

            return instance;
        }
    }
}
