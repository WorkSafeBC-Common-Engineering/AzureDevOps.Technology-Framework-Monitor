using ProjectData;
using ProjectData.Interfaces;

using System.Diagnostics;
using System.Xml;

namespace VisualStudioMetricsScanner
{
    public class MetricsScanner : IMetricsScanner
    {
        #region Private Members

        private const string metricsFile = "metrics.xml";

        private const string metricsExe = @"Metrics\Metrics.exe";

        private string metricsTargetFile = string.Empty;

        private const int readFileRetries = 5;

        #endregion

        #region IMetricsScanner Implementation

        Dictionary<string, Metrics> IMetricsScanner.Get(FileItem file, string basePath)
        {
            Console.WriteLine($"Getting metrics for {file.Path}");
            SetMetricsPath(basePath);

            GenerateMetricsFile($"{basePath}{file.Path}");

            var metrics = ParseMetricsFile();

            if (metrics.Count == 0)
            {
                Console.WriteLine($"\t>>> Unable to retrieving metrics");
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
                        Console.WriteLine($"Error reading metrics file: {ioException.Message} - retries left {retries}");
                        Thread.Sleep(1000);
                    }
                } while (retries-- >= 0);

                if (!readSuccessful)
                {
                    Console.WriteLine("Failed to read metrics file after multiple attempts.");
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
                Console.WriteLine($"Error getting metrics for current file: {ex}");
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
