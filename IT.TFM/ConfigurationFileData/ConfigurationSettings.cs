using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;

namespace ConfigurationFileData
{
    public static class ConfigurationSettings
    {
        public static string[] LoadConfigurationValues(string file, string section)
        {
            var configuration = GetConfiguration(file);
            return GetSectionValues(section, configuration);
        }

        public static Dictionary<string, string> LoadConfigurationDictionary(string file, string section)
        {
            var configuration = GetConfiguration(file);
            return GetSectionDictionary(section, configuration);
        }

        public static ConfigurationSection GetSection(string file, string sectionName)
        {
            var configuration = GetConfiguration(file);
            return configuration.GetSection(sectionName);
        }

        private static Configuration GetConfiguration(string file)
        {
            var configFileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = file
            };

            return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        private static string[] GetSectionValues(string sectionName, Configuration configuration)
        {
            var configData = GetSectionData(sectionName, configuration);

            var values = new List<string>();
            foreach (var key in configData.AllKeys)
            {
                values.Add(configData.Get(key));
            }

            return [.. values];
        }

        private static Dictionary<string, string> GetSectionDictionary(string sectionName, Configuration configuration)
        {
            var dictionaryData = new Dictionary<string, string>();

            var configData = GetSectionData(sectionName, configuration);

            foreach (var key in configData.AllKeys)
            {
                dictionaryData.Add(key, configData.Get(key));
            }

            return dictionaryData;
        }

        private static NameValueCollection GetSectionData(string sectionName, Configuration configuration)
        {
            var section = configuration.GetSection(sectionName);

            string sectionRawXml = section.SectionInformation.GetRawXml();
            var sectionXmlDoc = new XmlDocument();
            sectionXmlDoc.Load(new StringReader(sectionRawXml));

            var handler = new NameValueSectionHandler();
            var configParserSection = handler.Create(null, null, sectionXmlDoc.DocumentElement) as NameValueCollection;
            return configParserSection;
        }
    }
}
