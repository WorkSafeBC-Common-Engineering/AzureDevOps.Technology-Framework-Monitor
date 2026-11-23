using Microsoft.Extensions.Logging;

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
        private readonly ILogger<RuntimeMetricsScan> Logger;

        public RuntimeMetricsScan(ILogger<RuntimeMetricsScan> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            Logger = logger;
        }

        #region Public Methods

        public async Task Run(string projectId, string repositoryId, string[] excludedProjects)
        {
            Settings.Initialize();

            foreach (var name in Settings.Scanners)
            {
                var scanner = ScannerFactory.GetScanner(name);

                var organization = scanner.GetOrganization();

                await foreach (var project in scanner.Projects(projectId, excludedProjects))
                {
                    var repos = await scanner.Repositories(project, repositoryId);

                    foreach (var repo in repos)
                    {
                        Logger.LogInformation("Running Runtime Metrics Scan for {Organization} - {ProjectName} - {RepositoryName}", organization, project.Name, repo.Name);
                        var metricsScanner = ScannerFactory.GetRuntimeMetricsScanner(scanner, project.Id.ToString("D").ToLower(), repo.Id.ToString("D").ToLower(), repo.Name);
                        await metricsScanner.RunAsync();
                    }
                }
            }
        }

        #endregion
    }
}
