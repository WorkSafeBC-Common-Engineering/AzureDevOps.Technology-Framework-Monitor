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

        async Task IRuntimeMetricsScanner.Run()
        {
            var reader = StorageFactory.GetPipelineReader();
            //var scanner = ScannerFactory.GetScanner(token, organization, repositoryId);


            var pipelines = reader.GetPipelines(repositoryId);
            foreach (var pipeline in pipelines)
            {
                var logEntry = await GetLatestLogAsync(pipeline);
                if (logEntry != null)
                {
                    var logData = await GetDataAsync(logEntry);
                    var projectMetrics = ParseLogData(logData);

                    foreach (var metrics in projectMetrics)
                    {
                        var project = await FindProjectAsync(repositoryId, metrics.ProjectPath);
                        var fileMetrics = await GetMetricsAsync(project);

                        if (fileMetrics != null)
                        {
                            fileMetrics.LastRunTotalWarnings = metrics.TotalWarnings;
                            fileMetrics.LastRunTotalErrors = metrics.TotalErrors;

                            await SaveMetricsAsync(fileMetrics);
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task<LogEntry?> GetLatestLogAsync(Pipeline pipeline)
        {
            throw new NotImplementedException();
        }

        private async Task<string[]> GetDataAsync(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ProjectRuntimeMetrics> ParseLogData(string[] logData)
        {
            throw new NotImplementedException();
        }

        private async Task<Project> FindProjectAsync(string repositoryId, string projectPath)
        {
            throw new NotImplementedException();
        }

        private async Task<ProjectMetrics> GetMetricsAsync(Project project)
        {
            throw new NotImplementedException();
        }

        private async Task SaveMetricsAsync(ProjectMetrics fileMetrics)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
