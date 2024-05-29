using System;
using System.Collections.Generic;

using ConfigurationFileData;

using ProjectData;
using ProjectData.Interfaces;

namespace EndOfLifeConfigFile
{
    public class Checker : IEndOfLifeChecker
    {
        #region Private Members

        private const string configurationFile = "EndOfLifeConfigFile.dll.config";
        private const string configurationSection_DotNet = "EOLChecker-DotNet";
        private const string configurationSection_Component = "EOLChecker-Component";

        private static readonly Dictionary<string, DateTime> eolDotNetList;
        private static readonly Dictionary<string, DateTime> eolComponentList;

        #endregion

        static Checker()
        {
            eolDotNetList = LoadEol(configurationSection_DotNet);
            eolComponentList = LoadEol(configurationSection_Component);
        }

        #region IEndOfLifeChecker

        DateTime? IEndOfLifeChecker.GetEndOfLifeDate(ComponentTypeEnum componentType, string version)
        {
            return GetDate(componentType, version);
        }

        bool IEndOfLifeChecker.IsEndOfLife(ComponentTypeEnum componentType, string version)
        {
            var currentDate = DateTime.Now.Date;
            var eolDate = GetDate(componentType, version);

            return eolDate.HasValue && eolDate <= currentDate;
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, DateTime> LoadEol(string configurationSection)
        {
            var eolList = new Dictionary<string, DateTime>();

            var eolDictionaryValues = ConfigurationSettings.LoadConfigurationDictionary(configurationFile, configurationSection);
            foreach (var pair in eolDictionaryValues)
            {
                var dateValue = pair.Value;

                // If we cannot parse the date, ignore it
                if (DateTime.TryParse(dateValue, out DateTime date))
                {
                    eolList.Add(pair.Key, date);
                }
            }

            return eolList;
        }

        private static DateTime? GetDate(ComponentTypeEnum componentType, string version)
        {
            return componentType switch
            {
                ComponentTypeEnum.DotNet => GetDate(eolDotNetList, version),
                ComponentTypeEnum.Component => GetDate(eolComponentList, version),
                _ => null,
            };
        }

        private static DateTime? GetDate(Dictionary<string, DateTime> dictionary, string key)
        {
            return dictionary.TryGetValue(key, out DateTime value) ? value : new DateTime?();
        }

        #endregion
    }
}
