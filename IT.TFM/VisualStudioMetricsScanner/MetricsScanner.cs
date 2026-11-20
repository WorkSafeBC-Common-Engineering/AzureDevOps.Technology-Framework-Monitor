using Microsoft.Extensions.Logging;

using ProjectData;
using ProjectData.Interfaces;

using System.Diagnostics;
using System.Xml;

namespace VisualStudioMetricsScanner
{
    public class MetricsScanner : IMetricsScanner
    {
        #region Private Members

        private readonly ILogger<MetricsScanner> Logger;

        public MetricsScanner(ILogger<MetricsScanner> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            Logger = logger;
        }

        private const string metricsFile = "metrics.xml";

        private const string metricsExe = @"Metrics\Metrics.exe";

        private string metricsTargetFile = string.Empty;

        private const int readFileRetries = 5;

        #endregion

        #region IMetricsScanner Implementation

        Dictionary<string, Metrics> IMetricsScanner.Get(FileItem file, string basePath)
        {
            Logger.LogInformation("Getting metrics for {FilePath}", file.Path);
            SetMetricsPath(basePath);

            GenerateMetricsFile($"{basePath}{file.Path}");

            var metrics = ParseMetricsFile();

            if (metrics.Count == 0)
            {
                Logger.LogWarning("Unable to retrieve metrics for {FilePath}", file.Path);
            }

            return metrics;
        }

        #endregion

        #region Private Methods

        private void GenerateMetricsFile(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = metricsExe,
                Arguments = $"/q /s:{filePath} /o:{metricsTargetFile}"
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            if (process.Start())
            {
                process.WaitForExit();
            }
        }

        private Dictionary<string, Metrics> ParseMetricsFile()
        {
            var metricsList = new Dictionary<string, Metrics>();

            try
            {
                var xmlDoc = new XmlDocument();
                int retries = readFileRetries;
                bool readSuccessful = false;

                do
                {
                    try
                    {
                        xmlDoc.Load(metricsTargetFile);
                        readSuccessful = true;
                        break;
                    }
                    catch (IOException ioException)
                    {
                        Logger.LogWarning(ioException, "Error reading metrics file: {ErrorMessage} - retries left {RetriesLeft}", ioException.Message, retries);
                        Thread.Sleep(1000);
                    }
                } while (retries-- >= 0);

                if (!readSuccessful)
                {
                    Logger.LogError("Failed to read metrics file after multiple attempts.");
                    return metricsList;
                }

                var rootSolutionNodes = xmlDoc.SelectNodes("/CodeMetricsReport/Targets/Target");
                if (rootSolutionNodes == null)
                {
                    return metricsList;
                }

                foreach (XmlNode node in rootSolutionNodes)
                {
                    if (node == null || node.Attributes == null)
                    {
                        continue;
                    }

                    var projectPath = node.Attributes["Name"]?.Value;
                    if (projectPath == null)
                    {
                        continue;
                    }

                    var metricsNode = node.SelectSingleNode("Assembly/Metrics");
                    if (metricsNode == null)
                    {
                        continue;
                    }

                    var metrics = new Metrics();

                    foreach (XmlNode valueNode in metricsNode.ChildNodes)
                    {
                        if (valueNode.Attributes == null)
                        {
                            continue;
                        }

                        var nodeKey = valueNode.Attributes["Name"]?.Value ?? string.Empty;
                        var nodeValue = valueNode.Attributes["Value"]?.Value ?? string.Empty;
                        if (!int.TryParse(nodeValue, out var nodeIntValue))
                        {
                            nodeIntValue = 0;
                        }

                        switch (nodeKey)
                        {
                            case "MaintainabilityIndex":
                                metrics.MaintainabilityIndex = nodeIntValue;
                                break;

                            case "CyclomaticComplexity":
                                metrics.CyclomaticComplexity = nodeIntValue;
                                break;

                            case "ClassCoupling":
                                metrics.ClassCoupling = nodeIntValue;
                                break;

                            case "DepthOfInheritance":
                                metrics.DepthOfInheritance = nodeIntValue;
                                break;

                            case "SourceLines":
                                metrics.SourceLines = nodeIntValue;
                                break;

                            case "ExecutableLines":
                                metrics.ExecutableLines = nodeIntValue;
                                break;
                        }
                    }

                    metricsList[projectPath] = metrics;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting metrics for current file");
            }
            finally
            {
                if (File.Exists(metricsTargetFile))
                {
                    File.Delete(metricsTargetFile);
                }
            }

            return metricsList;
        }

        private void SetMetricsPath(string basePath)
        {
            var id = Path.GetRandomFileName();
            metricsTargetFile = $"{basePath}\\{id}.{metricsFile}";
        }

        #endregion
    }
}
