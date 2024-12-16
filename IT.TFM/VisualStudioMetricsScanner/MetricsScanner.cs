using ProjectData;
using ProjectData.Interfaces;

using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml;

namespace VisualStudioMetricsScanner
{
    public class MetricsScanner : IMetricsScanner
    {
        #region Private Members

        private const string metricsFile = "metrics.xml";

        private const string metricsExe = @"Metrics\Metrics.exe";

        #endregion

        #region IMetricsScanner Implementation

        Metrics? IMetricsScanner.Get(FileItem file, string basePath)
        {
            GenerateMetricsFile($"{basePath}{file.Path}");

            var metrics = ParseMetricsFile();

            return metrics;
        }

        #endregion

        #region Private Methods

        private static void GenerateMetricsFile(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = metricsExe,
                Arguments = $"/p:{filePath} /o:{metricsFile}"
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
                xmlDoc.Load(metricsFile);

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
                if (File.Exists(metricsFile))
                {
                    File.Delete(metricsFile);
                }
            }

            return null;
        }

        #endregion
    }
}
