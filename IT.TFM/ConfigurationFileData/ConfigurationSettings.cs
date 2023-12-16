using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;

using static System.Collections.Specialized.BitVector32;

namespace ConfigurationFileData
{
    public static class ConfigurationSettings
    {
        public static string[] LoadConfigurationValues(string file, string section)
        {
            System.Diagnostics.Debug.WriteLine($"=> LoadConfigurationValues: file={file}, section={section}");
            
            var configuration = GetConfiguration(file);

            System.Diagnostics.Debug.WriteLine($"=> LoadConfigurationValues: configuration loaded = {configuration != null}");

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

            System.Diagnostics.Debug.WriteLine($"=> GetSectionValues: configData loaded = {configData != null}");

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

            System.Diagnostics.Debug.WriteLine($"=> GetSectionData: section loaded = {section != null}");

            string sectionRawXml = section.SectionInformation.GetRawXml();

            System.Diagnostics.Debug.WriteLine($"=> GetSectionData: sectionRawXml = {sectionRawXml}");

            var sectionXmlDoc = new XmlDocument();
            sectionXmlDoc.Load(new StringReader(sectionRawXml));

            System.Diagnostics.Debug.WriteLine($"=> GetSectionData: sectionXmlDoc loaded");

            var handler = new NameValueSectionHandler();
            var configParserSection = handler.Create(null, null, sectionXmlDoc.DocumentElement) as NameValueCollection;

            System.Diagnostics.Debug.WriteLine($"=> GetSectionData: configParserSection loaded = {configParserSection != null}");

            return configParserSection;
        }
    }
}
