using ProjectData;
using ProjectData.Interfaces;

using System.Collections.Generic;

namespace FilterConfigurationFromConfigFile
{
    public class Configuration : IFilterConfiguration
    {
        #region Private Members

        private readonly char[] filterTypeSeparator = [':'];

        private const string configurationFile = "FilterConfigurationFromConfigFile.dll.config";
        private const string configurationSection = "FilterConfiguration";

        #endregion

        #region IFilterConfigurations Implementation

        FilterData[] IFilterConfiguration.Load()
        {
            var dataValues = ConfigurationFileData.ConfigurationSettings.LoadConfigurationValues(configurationFile, configurationSection);

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
