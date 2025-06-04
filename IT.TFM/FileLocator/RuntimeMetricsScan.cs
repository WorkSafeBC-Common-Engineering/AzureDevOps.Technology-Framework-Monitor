using ProjectScanner;

using RepoScan.DataModels;

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
            var repoReader = StorageFactory.GetRepoListReader();
            var repoList = repoReader.GetRepositoryIds();
            foreach (var id in repoList)
            {
                var scanner = ScannerFactory.GetRuntimeMetricsScanner(id);
                await scanner.Run();
            }
        }

        #endregion
    }
}
