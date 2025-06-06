using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System.Configuration;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RuntimeMetricsScanner
{
    public class Scanner : IRuntimeMetricsScanner
    {
        #region Private Members

        private string token = string.Empty;
        private string organization = string.Empty;

        private string projectId = string.Empty;
        private string repositoryId = string.Empty;
        private string repositoryName = string.Empty;
        private IScanner? scanner;

        private const string searchTagError = "Error(s)";
        private const string searchTagWarning = "Warning(s)";
        private const string searchCSharpProject = ".csproj";
        private const string searchVBProject = ".vbproj";

        #endregion

        #region IRuntimeMetricsScanner Implementation

        void IRuntimeMetricsScanner.Initialize(IScanner scanner, string projectId, string repositoryId, string repositoryName)
        {
            // Initialize the scanner with necessary settings
            token = ConfidentialSettings.Values.Token;
            organization = ConfidentialSettings.Values.Organization;

            this.projectId = projectId;
            this.repositoryId = repositoryId;
            this.repositoryName = repositoryName;
            this.scanner = scanner;
        }

        async Task IRuntimeMetricsScanner.RunAsync()
        {
            var reader = StorageFactory.GetPipelineReader();

            var pipelines = reader.GetPipelines(repositoryId);
            foreach (var pipeline in pipelines)
            {
                if (pipeline.RunId == null)
                {
                    continue;
                }

                var runId = pipeline.RunId.Value;

                var logEntries = await GetLogsAsync(pipeline, runId);
                if (logEntries == null)
                {
                    continue; // No log entry found for this pipeline run
                }

                var projectMetrics = await ParseLogEntriesAsync(pipeline, runId, logEntries);

                var groupedMetrics = projectMetrics
                    .GroupBy(metrics => metrics.ProjectPath.ToLowerInvariant()) // Group by ProjectPath (case-insensitive)
                    .Select(group => new ProjectRuntimeMetrics
                    {
                        ProjectPath = group.Key, // Use the grouped key as ProjectPath
                        TotalWarnings = group.Max(metrics => metrics.TotalWarnings), // Maximum TotalWarnings in the group
                        TotalErrors = group.Max(metrics => metrics.TotalErrors)      // Maximum TotalErrors in the group
                    });

                foreach (var metrics in groupedMetrics)
                {
                    await SaveMetricsAsync(repositoryId, metrics );
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task<IEnumerable<PipelineRunLog>> GetLogsAsync(Pipeline pipeline, int runId)
        {
            if (scanner == null)
            {
                throw new InvalidOperationException("IRunTimeMetricsScanner: Initialize was not called.");
            }

            var logEntries = await scanner.PipelineRunLogs(projectId, pipeline.Id, runId);
            if (logEntries == null)
            {
                return [];
            }

            return logEntries;
        }

        private async Task<IEnumerable<ProjectRuntimeMetrics>> ParseLogEntriesAsync(Pipeline pipeline, int runId, IEnumerable<PipelineRunLog> logEntries)
        {
            var logMetrics = new List<ProjectRuntimeMetrics>();

            foreach (var logEntry in logEntries)
            {
                var logData = await GetDataAsync(pipeline, runId, logEntry);

                var metrics = ParseLogData(logData);
                if (!metrics.Any())
                {
                    continue;
                }

                logMetrics.AddRange(metrics);
            }

            return logMetrics.Distinct().AsEnumerable();
        }

        private async Task<string[]> GetDataAsync(Pipeline pipeline, int runId, PipelineRunLog logEntry)
        {
            if (scanner == null)
            {
                throw new InvalidOperationException("IRunTimeMetricsScanner: Initialize was not called.");
            }

            var logContent = await scanner.PipelineRunLogContent(projectId, pipeline.Id, runId, logEntry.Id);

            // Split the log content into a string array by newlines
            var logData = logContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

            return logData;
        }

        private IEnumerable<ProjectRuntimeMetrics> ParseLogData(string[] logData)
        {
            var logEntryMetrics = new List<ProjectRuntimeMetrics>();

            for (int index = 0; index < logData.Length; index++)
            {
                var entry = logData[index];
                if (entry.Contains(searchTagError))
                {
                    var metrics = GetMetricsData(logData, index);
                    if (metrics != null)
                    {
                        logEntryMetrics.Add(metrics);
                    }
                }
            }

            var uniqueMetrics = logEntryMetrics.Distinct();

            return uniqueMetrics.AsEnumerable();
        }

        private ProjectRuntimeMetrics? GetMetricsData(string[] logData, int tagIndex)
        {
            var currentIndex = tagIndex - 1;

            var lineWarnings = logData[currentIndex];
            var lineErrors = logData[tagIndex];
            var lineProject = string.Empty;

            if (!lineWarnings.Contains(searchTagWarning))
            {
                // must be the immediate predecessor to the error tag
                return null;
            }

            while (--currentIndex >= 0)
            {
                var currentLine = logData[currentIndex];
                if (currentLine.Contains(searchTagError))
                {
                    // If we hit the previous error tag line, then we have completed the parsing of this section of the log file.
                    break;
                }

                // search for the line containing the project file
                if (currentLine.Contains(searchCSharpProject, StringComparison.InvariantCultureIgnoreCase) ||
                    currentLine.Contains(searchVBProject, StringComparison.InvariantCultureIgnoreCase))
                {
                    lineProject = currentLine;
                    break;
                }
            }

            if (lineWarnings == string.Empty && lineProject == string.Empty)
            {
                return null;
            }

            var warningsValue = ParseMetricsValue(lineWarnings, searchTagWarning);
            var errorsValue = ParseMetricsValue(lineErrors, searchTagError);
            var projectValue = ParseMetricsProject(lineProject);

            if (warningsValue >= 0 && errorsValue >= 0 && !string.IsNullOrEmpty(projectValue))
            {
                return new ProjectRuntimeMetrics { ProjectPath = projectValue, TotalWarnings = warningsValue, TotalErrors = errorsValue };
            }

            return null;

        }

        private static int ParseMetricsValue(string metricsLine, string metricsType)
        {
            var index = metricsLine.IndexOf(metricsType);
            var subContent = metricsLine[..index];
            var fields = subContent.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (fields == null)
            {
                return -1;
            }

            if (!int.TryParse(fields[^1], out var metricsValue))
            {
                return -1;
            }

            return metricsValue;
        }

        private string ParseMetricsProject(string metricsLine)
        {
            int index;
            if (metricsLine.Contains(searchCSharpProject, StringComparison.InvariantCultureIgnoreCase))
            {
                index = metricsLine.IndexOf(searchCSharpProject, StringComparison.InvariantCultureIgnoreCase) + searchCSharpProject.Length;
            }
            else if (metricsLine.Contains(searchVBProject, StringComparison.InvariantCultureIgnoreCase))
            {
                index = metricsLine.IndexOf(searchVBProject, StringComparison.InvariantCultureIgnoreCase) + searchVBProject.Length;
            }
            else
            {
                return string.Empty;
            }

            // trim any content after the project file.
            var subContent = metricsLine[..index];
            var fields = subContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (fields == null)
            {
                return string.Empty;
            }

            var projectField = fields[^1];
            index = projectField.IndexOf('/');
            if (index == -1)
            {
                index = projectField.IndexOf('\\');
            }

            if (index == -1)
            {
                return string.Empty;
            }

            // Need to strip out the parts of the path that are related to the build agent local directories.
            // So we just want the path after the repository name
            var projectPath = projectField[index..];
            index = projectPath.IndexOf(repositoryName, StringComparison.InvariantCultureIgnoreCase);

            if (index == -1)
            {
                return string.Empty;
            }

            return projectPath[(index + repositoryName.Length)..];
        }

        private static async Task SaveMetricsAsync(string repositoryId, ProjectRuntimeMetrics metrics)
        {
            var filePath = metrics.ProjectPath.Replace('\\', '/');
            var fileItem = new ProjectData.FileItem { RepositoryId = new Guid(repositoryId), Path = filePath };
            var writer = Storage.StorageFactory.GetStorageWriter();
            await writer.SaveMetricsAsync(fileItem, null, metrics);
        }

        #endregion
    }
}
