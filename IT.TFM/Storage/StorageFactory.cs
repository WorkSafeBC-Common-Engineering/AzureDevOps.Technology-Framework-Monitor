using Microsoft.Practices.Unity.Configuration;

using ProjectData.Interfaces;

using System.Configuration;

using Unity;

namespace Storage
{
    public static class StorageFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "StorageContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static StorageFactory()
        {
            _container = new UnityContainer().AddExtension(new Diagnostic());
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        public static IStorageWriter GetStorageWriter()
        {
            var instance = _container.Resolve<IStorageWriter>(Settings.Storage);
            instance.Initialize(Settings.Configuration);

            return instance;
        }

        public static IStorageReader GetStorageReader()
        {
            var instance = _container.Resolve<IStorageReader>(Settings.Storage);
            instance.Initialize(Settings.Configuration);

            return instance;
        }
    }
}
