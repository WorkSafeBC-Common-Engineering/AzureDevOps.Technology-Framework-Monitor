using System.Configuration;

namespace Storage
{
    public static class Settings
    {
        #region Private Members

        private const string storageKey = "StorageType";
        private const string configurationKey = "StorageConfiguration";

        #endregion

        #region Constructors

        static Settings()
        {
            var appSettings = ConfigurationManager.AppSettings;
            Storage = appSettings[storageKey];

            var dbConfiguration = ConfidentialSettings.Values.DbConnection;
            if (string.IsNullOrEmpty(dbConfiguration))
            {
                Configuration = $"name={appSettings[configurationKey]}";
            }
            else
            {
                Configuration = dbConfiguration;
            }
        }

        #endregion

        #region Public Properties

        internal static string Storage { get; private set; }

        internal static string Configuration { get; private set; }

        #endregion
    }
}
