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

        public static async Task Run(string projectId, string repositoryId)
        {
            var repoReader = StorageFactory.GetRepoListReader();
            Settings.Initialize();

            foreach (var name in Settings.Scanners)
            {
                var scanner = ScannerFactory.GetScanner(name);

                var organization = scanner.GetOrganization();

                await foreach (var project in scanner.Projects(projectId))
                {
                    var repos = await scanner.Repositories(project, repositoryId);

                    foreach (var repo in repos)
                    {
                        var metricsScanner = ScannerFactory.GetRuntimeMetricsScanner(repo.Id.ToString());
                        await metricsScanner.Run();
                    }
                }
            }

        }

        #endregion
    }
}
