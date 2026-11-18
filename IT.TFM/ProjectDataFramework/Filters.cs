using ProjectData.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectData
{
    public class Filters
    {
        #region Private Members

        private const string contentFilter = "Content";

        private readonly List<IFilter> filters = [];

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

            return [.. results];
        }

        #endregion

        #region Private Methods

        private static IFilter GetFilterInstance(FilterData data)
        {
            var filter = ServiceProviderAccessor.GetRequiredKeyedService<IFilter>(data.FilterType);
            filter.Initialize(data.Data);

            return filter;
        }

        #endregion
    }
}
