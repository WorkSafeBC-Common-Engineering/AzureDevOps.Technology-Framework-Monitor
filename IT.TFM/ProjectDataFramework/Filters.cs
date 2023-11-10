using Microsoft.Practices.Unity.Configuration;
using ProjectData.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ProjectData
{
    public class Filters
    {
        #region Private Members

        private const string configurationSection = "unity";
        private const string dependencyInjectionContainer = "FilterContainer";
        private const string contentFilter = "Content";

        private static readonly IUnityContainer _container;

        private readonly List<IFilter> filters = new();

        #endregion

        #region Constructors

        static Filters()
        {
            _container = new UnityContainer().AddExtension(new Diagnostic());
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configurationSection);
            section.Configure(_container, dependencyInjectionContainer);
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            var data = FilterConfiguration.Configuration;

            foreach (var item in data)
            {
                // Only do content filters here
                if (item.FilterType.Equals(contentFilter, StringComparison.InvariantCultureIgnoreCase))
                {
                    filters.Add(GetFilterInstance(item));
                }
            }
        }

        public IEnumerable<string> ColumnNames
        {
            get { return filters.Select(f => f.ColumnName).AsEnumerable(); }
        }

        public bool[] Match(string fields)
        {
            var results = new List<bool>();

            foreach (var filter in filters)
            {
                results.Add(filter.IsMatch(fields));
            }

            return results.ToArray();
        }

        #endregion

        #region Private Methods

        private static IFilter GetFilterInstance(FilterData data)
        {
            var filter = _container.Resolve<IFilter>(data.FilterType);
            filter.Initialize(data.Data);

            return filter;
        }

        #endregion
    }
}
