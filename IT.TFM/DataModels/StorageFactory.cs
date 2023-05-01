using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;


namespace RepoScan.DataModels
{
    public static class StorageFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "RepoStorageContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static StorageFactory()
        {
            _container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        public static IReadFileDetails GetFileDetailsReader()
        {
            return _container.Resolve<IReadFileDetails>();
        }

        public static IReadFileItem GetFileItemReader()
        {
            return _container.Resolve<IReadFileItem>();
        }

        public static IReadRepoList GetRepoListReader()
        {
            return _container.Resolve<IReadRepoList>();
        }

        public static IWriteFileDetails GetFileDetailsWriter()
        {
            return _container.Resolve<IWriteFileDetails>();
        }

        public static IWriteFileItem GetFileItemWriter()
        {
            return _container.Resolve<IWriteFileItem>();
        }

        public static IWriteRepoList GetRepoListWriter()
        {
            return _container.Resolve<IWriteRepoList>();
        }
    }
}
