using Microsoft.Practices.Unity.Configuration;
using ProjectData.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Parser
{
    public class EndOfLifeCheckerFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "EndOfLifeCheckContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static EndOfLifeCheckerFactory()
        {
            _container = new UnityContainer().AddExtension(new Diagnostic());
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        #region Factory Methods

        public static IEndOfLifeChecker Get()
        {
            return _container.Resolve<IEndOfLifeChecker>();
        }

        #endregion
    }
}
