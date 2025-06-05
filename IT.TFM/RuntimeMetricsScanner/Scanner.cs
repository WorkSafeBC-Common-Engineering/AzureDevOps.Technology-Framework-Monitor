using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

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
            var reader = StorageFactory.GetPipelineReader();
            //var scanner = ScannerFactory.GetScanner(token, organization, repositoryId);


            var pipelines = reader.GetPipelines(repositoryId);
            foreach (var pipeline in pipelines)
            {
                var logEntry = GetLatestLog(pipeline);
                if (logEntry != null)
                {
                    var logData = GetData(logEntry);
                    var projectMetrics = ParseLogData(logData);

                    foreach (var metrics in projectMetrics)
                    {
                        var project = FindProject(repositoryId, metrics.ProjectPath);
                        var fileMetrics = GetMetrics(project);

                        if (fileMetrics != null)
                        {
                            fileMetrics.LastRunTotalWarnings = metrics.TotalWarnings;
                            fileMetrics.LastRunTotalErrors = metrics.TotalErrors;
                        }
                    }
                }
            }

            throw new NotImplementedException("Runtime metrics scanning is not yet implemented.");
        }

        #endregion

        #region Private Methods

        private LogEntry? GetLatestLog(Pipeline pipeline)
        {
            throw new NotImplementedException();
        }

        private string[] GetData(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ProjectRuntimeMetrics> ParseLogData(string[] logData)
        {
            throw new NotImplementedException();
        }

        private Project FindProject(string repositoryId, string projectPath)
        {
            throw new NotImplementedException();
        }

        private ProjectMetrics GetMetrics(Project project)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
