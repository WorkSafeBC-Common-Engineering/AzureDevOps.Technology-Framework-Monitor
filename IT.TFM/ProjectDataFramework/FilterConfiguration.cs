using ProjectData.Interfaces;

namespace ProjectData
{
    public static class FilterConfiguration
    {
        #region Constructors

        static FilterConfiguration()
        {
            var configuration = ServiceProviderAccessor.GetRequiredService<IFilterConfiguration>();
            Configuration = configuration.Load();
        }

        #endregion

        #region Public Properties and Methods

        public static FilterData[] Configuration { get; private set; }

        #endregion
    }
}
