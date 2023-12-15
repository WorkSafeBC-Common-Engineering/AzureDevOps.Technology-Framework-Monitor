namespace ConfidentialSettings
{
    public static class Values
    {
        #region Private Members

        private const string envToken = "azdo-pat";

        private const string envOrg = "azdo-org-name";

        private const string envUrl = "azdo-org-url";

        private const string envDbConnection = "sql-connectionstring";

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
