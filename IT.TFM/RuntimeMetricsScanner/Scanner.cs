using ProjectData.Interfaces;

namespace RuntimeMetricsScanner
{
    public class Scanner : IRuntimeMetricsScanner
    {
        #region Private Members

        private string token = string.Empty;
        private string organization = string.Empty;
        
        #endregion

        #region IRuntimeMetricsScanner Implementation
        
        void IRuntimeMetricsScanner.Initialize()
        {
            // Initialize the scanner with necessary settings
            token = ConfidentialSettings.Values.Token;
            organization = ConfidentialSettings.Values.Organization;
        }
        
        #endregion

        #region Private Methods
        
        // Add methods to scan runtime metrics here
        
        #endregion
    }
}
