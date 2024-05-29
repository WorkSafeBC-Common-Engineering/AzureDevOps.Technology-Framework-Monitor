using ProjectData.Interfaces;

using System;

namespace ContentFilter
{
    public class Filter : IFilter
    {
        #region Private Members

        private readonly char[] fieldSeparator = ['|'];

        private string columnName;
        private string filterValue;

        #endregion

        #region IFilter Implementation

        void IFilter.Initialize(string data)
        {
            var fields = data.Split(fieldSeparator, StringSplitOptions.RemoveEmptyEntries);

            columnName = fields[0].Trim();
            filterValue = fields[1].Trim().ToLower();
        }

        string IFilter.ColumnName => columnName;

        bool IFilter.IsMatch(string data)
        {
            return data.Contains(filterValue, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}
