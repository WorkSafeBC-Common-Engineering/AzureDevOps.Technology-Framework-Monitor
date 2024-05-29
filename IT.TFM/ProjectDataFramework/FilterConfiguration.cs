using Microsoft.Practices.Unity.Configuration;

using ProjectData.Interfaces;

using System.Configuration;

using Unity;

namespace ProjectData
{
    public static class FilterConfiguration
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "FilterContainer";

        #endregion

        #region Constructors

        static FilterConfiguration()
        {
            var container = new UnityContainer().AddExtension(new Diagnostic());
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(container, dependencyInjectionContainer);

            var configuration = container.Resolve<IFilterConfiguration>();
            Configuration = configuration.Load();
        }

        #endregion

        #region Public Properties and Methods

        public static FilterData[] Configuration { get; private set; }

        #endregion
    }
}
