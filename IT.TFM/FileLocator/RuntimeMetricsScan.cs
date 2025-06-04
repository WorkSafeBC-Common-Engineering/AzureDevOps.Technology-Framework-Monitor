using ProjectScanner;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public class RuntimeMetricsScan
    {
        #region Public Methods

        public static async Task Run()
        {
            var scanner = ScannerFactory.GetRuntimeMetricsScanner();
        }

        #endregion
    }
}
