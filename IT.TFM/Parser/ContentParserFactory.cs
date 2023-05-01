using Microsoft.Practices.Unity.Configuration;
using Parser.Interfaces;
using ProjectData;
using System.Configuration;
using Unity;

namespace Parser
{
    public static class ContentParserFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "ContentParserContainer";

        private static readonly IUnityContainer _container;

        #endregion

        #region Constructors

        static ContentParserFactory()
        {
            _container = new UnityContainer().AddExtension(new Diagnostic());
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        #region Factory Methods

        public static IContentParser Get(string itemType)
        {
            return _container.Resolve<IContentParser>(itemType);
        }

        #endregion
    }
}
