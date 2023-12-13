namespace ConfidentialSettings
{
    public static class Values
    {
        #region Private Members

        private const string envToken = "TFM_AdToken";

        private const string envOrg = "TFM_ScannerName";

        private const string envUrl = "TFM_ScannerValue";

        private const string envDbConnection = "TFM_DbConnection";

        #endregion

        #region Constructors

        static Values()
        {
            Token = Environment.GetEnvironmentVariable(envToken) ?? string.Empty;
            Organization = Environment.GetEnvironmentVariable(envOrg) ?? string.Empty;
            OrganizationUrl = Environment.GetEnvironmentVariable(envUrl) ?? string.Empty;
            DbConnection = Environment.GetEnvironmentVariable(envDbConnection) ?? string.Empty;
        }

        #endregion

        #region Public Properties

        public static string Token { get; private set; } = string.Empty;

        public static string Organization { get; private set; } = string.Empty;

        public static string OrganizationUrl { get; private set; } = string.Empty;

        public static string DbConnection { get; private set; } = string.Empty;

        #endregion

    }
}
