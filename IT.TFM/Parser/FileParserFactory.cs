using Microsoft.Practices.Unity.Configuration;
using Parser.Interfaces;
using ProjectData;
using System.Configuration;
using Unity;

namespace Parser
{
    public static class FileParserFactory
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "FileParserContainer";

        private static readonly IUnityContainer _container = new UnityContainer().AddExtension(new Diagnostic());

        #endregion

        #region Constructors

        static FileParserFactory()
        {
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        #region Factory Methods

        public static IFileParser Get(FileItemType itemType)
        {
            return _container.Resolve<IFileParser>(itemType.ToString());
        }

        #endregion
    }
}
