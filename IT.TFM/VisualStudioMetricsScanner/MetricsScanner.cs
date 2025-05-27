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

        Metrics? IMetricsScanner.Get(FileItem file, string basePath)
        {
            Console.WriteLine($"Getting metrics for {file.Path}");
            SetMetricsPath(basePath, file);

            GenerateMetricsFile($"{basePath}{file.Path}");

            var metrics = ParseMetricsFile();

            if (metrics == null)
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
                Arguments = $"/p:{filePath} /o:{metricsTargetFile}"
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

        private Metrics? ParseMetricsFile()
        {
            Metrics? metrics = null;

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
                    return metrics;
                }

                var rootNode = xmlDoc.SelectSingleNode("/CodeMetricsReport/Targets/Target/Assembly/Metrics");
                if (rootNode == null)
                {
                    return metrics;
                }
                metrics = new Metrics();

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes == null)
                    {
                        return null;
                    }

                    var nodeKey = node.Attributes["Name"]?.Value ?? string.Empty;
                    var nodeValue = node.Attributes["Value"]?.Value ?? string.Empty;
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

                return metrics;
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

            return null;
        }

        private void SetMetricsPath(string basePath, FileItem file)
        {
            metricsTargetFile = $"{basePath}\\{file.Id}.{metricsFile}";
        }

        #endregion
    }
}
