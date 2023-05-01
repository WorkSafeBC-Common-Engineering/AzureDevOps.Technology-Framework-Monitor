using Microsoft.VisualStudio.Services.Common;
using ScannerCommon.Interfaces;

namespace TokenCredentials
{
    public class Credentials : ICredentials
    {
        private const string tokenVariable = "TFM_AdToken";

        VssCredentials ICredentials.Get()
        {
            var credentials = System.Environment.GetEnvironmentVariable(tokenVariable);

            return new VssBasicCredential(string.Empty, credentials);
        }
    }
}
