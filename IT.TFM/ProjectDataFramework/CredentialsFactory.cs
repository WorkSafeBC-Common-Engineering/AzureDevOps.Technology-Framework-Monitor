using Microsoft.Practices.Unity.Configuration;
using ProjectData.Interfaces;
using ScannerCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ProjectData
{
    public static class CredentialsFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "CredentialsContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static CredentialsFactory()
        {
            _container = new UnityContainer().AddExtension(new Diagnostic());
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        public static ICredentials GetCredentialsManager()
        {
            return _container.Resolve<ICredentials>();
        }
    }
}
