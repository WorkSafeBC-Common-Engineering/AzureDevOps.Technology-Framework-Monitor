using ProjectData;
using ProjectData.Interfaces;

using System.Collections.Generic;

namespace FilterConfigurationFromConfigFile
{
    public class Configuration : IFilterConfiguration
    {
        private readonly ConfigurationFileData.ConfigurationSettings configurationSettings;

        public Configuration(ConfigurationFileData.ConfigurationSettings configurationSettings)
        {
            System.ArgumentNullException.ThrowIfNull(configurationSettings);
            this.configurationSettings = configurationSettings;
        }

        #region Private Members

        private readonly char[] filterTypeSeparator = [':'];

        private const string configurationFile = "FilterConfigurationFromConfigFile.dll.config";
        private const string configurationSection = "FilterConfiguration";

        #endregion

        #region IFilterConfigurations Implementation

        FilterData[] IFilterConfiguration.Load()
        {
            var dataValues = configurationSettings.LoadConfigurationValues(configurationFile, configurationSection);

            var data = new List<FilterData>(dataValues.Length);

            foreach (var item in dataValues)
            {
                var fields = item.Split(filterTypeSeparator);
                data.Add(new FilterData { FilterType = fields[0], Data = fields[1] });
            }

            return [.. data];
        }

        #endregion
    }
}
