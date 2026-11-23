using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;

using Microsoft.Extensions.Logging;

namespace ConfigurationFileData
{
    public class ConfigurationSettings
    {
        private readonly ILogger<ConfigurationSettings> Logger;

        public ConfigurationSettings(ILogger<ConfigurationSettings> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            Logger = logger;
        }

        public string[] LoadConfigurationValues(string file, string section)
        {
            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("LoadConfigurationValues: file={File}, section={Section}", file, section);
            }

            var configuration = GetConfiguration(file);

            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("LoadConfigurationValues: configuration loaded = {ConfigurationLoaded}", configuration != null);
            }

            if ( configuration != null )
            {
                if (Parameters.Settings.ExtendedLogging)
                {
                    Logger.LogInformation("LoadConfigurationValues: configuration has file = {HasFile}", configuration.HasFile);
                    Logger.LogInformation("LoadConfigurationValues: configuration file path = {FilePath}", configuration.FilePath);
                    Logger.LogInformation("LoadConfigurationValues: configuration file exists = {FileExists}", File.Exists(configuration.FilePath));
                    Logger.LogInformation("LoadConfigurationValues: configuration file path = {FilePath}", configuration.FilePath);
                }

                foreach (string key in configuration.Sections.Keys)
                {
                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("LoadConfigurationValues: configuration section key = {Key}", key);
                    }

                    var value = configuration.Sections[key];

                    if (Parameters.Settings.ExtendedLogging)
                    {
                        Logger.LogInformation("LoadConfigurationValues: configuration section type = {SectionType}", value.GetType());
                    }
                }
            }

            return GetSectionValues(section, configuration);
        }

        public Dictionary<string, string> LoadConfigurationDictionary(string file, string section)
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
            var configPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            var configFileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = Path.Combine(configPath, file)
            };

            return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        private string[] GetSectionValues(string sectionName, Configuration configuration)
        {
            var configData = GetSectionData(sectionName, configuration);

            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("GetSectionValues: configData loaded = {ConfigDataLoaded}", configData != null);
            }

            var values = new List<string>();
            foreach (var key in configData.AllKeys)
            {
                values.Add(configData.Get(key));
            }

            return [.. values];
        }

        private Dictionary<string, string> GetSectionDictionary(string sectionName, Configuration configuration)
        {
            var dictionaryData = new Dictionary<string, string>();

            var configData = GetSectionData(sectionName, configuration);

            foreach (var key in configData.AllKeys)
            {
                dictionaryData.Add(key, configData.Get(key));
            }

            return dictionaryData;
        }

        private NameValueCollection GetSectionData(string sectionName, Configuration configuration)
        {
            var section = configuration.GetSection(sectionName);

            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("GetSectionData: section loaded = {SectionLoaded}", section != null);
            }

            string sectionRawXml = section.SectionInformation.GetRawXml();

            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("GetSectionData: sectionRawXml = {SectionRawXml}", sectionRawXml);
            }

            var sectionXmlDoc = new XmlDocument();
            sectionXmlDoc.Load(new StringReader(sectionRawXml));

            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("GetSectionData: sectionXmlDoc loaded");
            }

            var handler = new NameValueSectionHandler();
            var configParserSection = handler.Create(null, null, sectionXmlDoc.DocumentElement) as NameValueCollection;

            if (Parameters.Settings.ExtendedLogging)
            {
                Logger.LogInformation("GetSectionData: configParserSection loaded = {ConfigParserSectionLoaded}", configParserSection != null);
            }

            return configParserSection;
        }
    }
}
