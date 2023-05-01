using Microsoft.VisualStudio.Services.Common;

namespace ScannerCommon.Interfaces
{
    public interface ICredentials
    {
        VssCredentials Get();
    }
}
