using System;
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
            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> LoadConfigurationValues: file={file}, section={section}");
            }

            var configuration = GetConfiguration(file);

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> LoadConfigurationValues: configuration loaded = {configuration != null}");
            }

            if ( configuration != null )
            {
                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"=> LoadConfigurationValues: configuration has file = {configuration.HasFile}");
                    Console.WriteLine($"=> LoadConfigurationValues: configuration file path = {configuration.FilePath}");
                    Console.WriteLine($"=> LoadConfigurationValues: configuration file exists = {File.Exists(configuration.FilePath)}");
                    Console.WriteLine($"=> LoadConfigurationValues: configuration file path = {configuration.FilePath}");
                }

                foreach (string key in configuration.Sections.Keys)
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($"\t=> LoadConfigurationValues: configuration section key = {key}:");
                    }

                    var value = configuration.Sections[key];

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Console.WriteLine($"\t=> LoadConfigurationValues: configuration section type = {value.GetType()}:");
                    }
                }
            }

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

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> GetSectionValues: configData loaded = {configData != null}");
            }

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

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> GetSectionData: section loaded = {section != null}");
            }

            string sectionRawXml = section.SectionInformation.GetRawXml();

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> GetSectionData: sectionRawXml = {sectionRawXml}");
            }

            var sectionXmlDoc = new XmlDocument();
            sectionXmlDoc.Load(new StringReader(sectionRawXml));

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> GetSectionData: sectionXmlDoc loaded");
            }

            var handler = new NameValueSectionHandler();
            var configParserSection = handler.Create(null, null, sectionXmlDoc.DocumentElement) as NameValueCollection;

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"=> GetSectionData: configParserSection loaded = {configParserSection != null}");
            }

            return configParserSection;
        }
    }
}
