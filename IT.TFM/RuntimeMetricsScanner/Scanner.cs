using ProjectData.Interfaces;

namespace RuntimeMetricsScanner
{
    public class Scanner : IRuntimeMetricsScanner
    {
        #region Private Members

        private string token = string.Empty;
        private string organization = string.Empty;
        private string repositoryId = string.Empty;

        #endregion

        #region IRuntimeMetricsScanner Implementation

        void IRuntimeMetricsScanner.Initialize(string repositoryId)
        {
            // Initialize the scanner with necessary settings
            token = ConfidentialSettings.Values.Token;
            organization = ConfidentialSettings.Values.Organization;
            this.repositoryId = repositoryId;
        }

        Task IRuntimeMetricsScanner.Run()
        {



            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        // Add methods to scan runtime metrics here

        #endregion
    }
}
