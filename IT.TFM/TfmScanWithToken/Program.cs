using Microsoft.Extensions.DependencyInjection;

using ProjectData;
using RepoScan.FileLocator;

using System;
using System.Configuration;
using System.Threading.Tasks;

using RepoFileScan = RepoScan.FileLocator;

namespace TfmScanWithToken
{
    internal static class Program
    {
        #region Private Members

        private static bool partOne = false;
        private static bool partTwo = false;
        private static bool partThree = false;
        private static bool partFour = false;
        private static bool partFive = false;
        private static bool partSix = false;

        #endregion

        static async Task Main(string[] args)
        {
            using var provider = ServiceRegistration.CreateProvider();
            ServiceProviderAccessor.Initialize(provider);

            var threadCount = GetTotalThreads(args);
            var forceDetails = GetForceDetails(args);
            var excludedProjects = GetExclusions(args);
            GetParts(args);
            GetExtendedLogging(args);

            var projectId = GetProjectId(args);
            var repositoryId = GetRepositoryId(args);

            if (partOne)
            {
                await RepoScanAsync(provider.GetRequiredService<Scanner>(), projectId, repositoryId, excludedProjects);
            }

            if (partTwo)
            {
                await FileScanAsync(provider.GetRequiredService<FileProcessor>(), threadCount, forceDetails, projectId, repositoryId, excludedProjects);
            }

            if (partThree)
            {
                PipelineScan(provider.GetRequiredService<RepoFileScan.PipelineScanner>(), threadCount, repositoryId);
            }

            if (partFour)
            {
                await FileDetailsAsync(provider.GetRequiredService<RepoFileScan.FileDetails>(), threadCount, forceDetails, projectId, repositoryId, excludedProjects);
            }

            if (partFive)
            {
                await NuGetFeedScan();
            }

            if (partSix)
            {
                await GetRuntimeMetrics(provider.GetRequiredService<RuntimeMetricsScan>(), projectId, repositoryId, excludedProjects);
            }

#if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
#endif
        }

        private static async Task RepoScanAsync(Scanner scanner, string projectId, string repositoryId, string[] excludedProjects)
        {
            Console.WriteLine($"Starting Repo Scan at: {DateTime.Now:T}");

            await scanner.ScanAsync(projectId, repositoryId, excludedProjects);

            Console.WriteLine($"Repo Scan complete at: {DateTime.Now:T}");
        }

        private static async Task FileScanAsync(FileProcessor fileProcessor, int threadCount, bool forceDetails, string projectId, string repositoryId, string[] excludedProjects)
        {
            Console.WriteLine($"Starting File Scan at: {DateTime.Now:T}");

            await fileProcessor.GetFiles(threadCount, forceDetails, projectId, repositoryId, excludedProjects);

            Console.WriteLine($"File Scan complete at: {DateTime.Now:T}");
        }

        private static void PipelineScan(RepoFileScan.PipelineScanner pipelineScanner, int threadCount, string repositoryId)
        {
            Console.WriteLine($"Starting Pipeline Scan at: {DateTime.Now:T}");

            pipelineScanner.LinkPipelineToFile(threadCount, repositoryId);

            Console.WriteLine($"Pipeline Scan complete at: {DateTime.Now:T}");
        }

        private static async Task FileDetailsAsync(RepoFileScan.FileDetails fileDetails, int threadCount, bool forceDetails, string projectId, string repositoryId, string[] excludedProjects)
        {
            Console.WriteLine($"Starting File Details Scan at: {DateTime.Now:T}");

            await fileDetails.GetDetailsAsync(threadCount, forceDetails, projectId, repositoryId, excludedProjects);

            Console.WriteLine($"File Details Scan complete at: {DateTime.Now:T}");
        }

        private static async Task NuGetFeedScan()
        {
            Console.WriteLine($"Starting NuGet Feed Scan at: {DateTime.Now:T}");

            await NuGetScan.Run();

            Console.WriteLine($"NuGet Feed Scan complete at: {DateTime.Now:T}");
        }

        private static async Task GetRuntimeMetrics(RuntimeMetricsScan runtimeMetricsScan, string projectId, string repositoryId, string[] excludedProjects)
        {
            Console.WriteLine($"Starting Runtime Metrics Scan at: {DateTime.Now:T}");

            await runtimeMetricsScan.Run(projectId, repositoryId, excludedProjects);

            Console.WriteLine($"Runtime Metrics Scan complete at: {DateTime.Now:T}");
        }

        private static int GetTotalThreads(string[] args)
        {
            var threadValue = GetCommandLineValue(args, "-t");
            if (string.IsNullOrWhiteSpace(threadValue))
            {
                threadValue = ConfigurationManager.AppSettings["processingThreads"];
            }

            if (string.IsNullOrWhiteSpace(threadValue) || !int.TryParse(threadValue, out var threadCount))
            {
                threadCount = 1;
            }

            return threadCount;
        }

        private static bool GetForceDetails(string[] args)
        {
            var forceDetailsValue = GetCommandLineValue(args, "-d");
            if (string.IsNullOrWhiteSpace(forceDetailsValue))
            {
                forceDetailsValue = ConfigurationManager.AppSettings["forceDetails"];
            }

            if (string.IsNullOrWhiteSpace(forceDetailsValue) || !bool.TryParse(forceDetailsValue, out var forceDetails))
            {
                forceDetails = false;
            }

            return forceDetails;
        }

        private static void GetParts(string[] args)
        {
            var partsValue = GetCommandLineValue(args, "-s");

            switch (partsValue)
            {
                case "1":
                    partOne = true;
                    break;
                case "2":
                    partTwo = true;
                    break;
                case "3":
                    partThree = true;
                    break;
                case "4":
                    partFour = true;
                    break;
                case "5":
                    partFive = true;
                    break;
                case "6":
                    partSix = true;
                    break;
                default:
                    partOne = true;
                    partTwo = true;
                    partThree = true;
                    partFour = true;
                    partFive = true;
                    partSix = false;
                    break;
            }
        }

        private static string GetProjectId(string[] args)
        {
            return GetCommandLineValue(args, "-p");
        }

        private static string GetRepositoryId(string[] args)
        {
            return GetCommandLineValue(args, "-r");
        }

        private static string[] GetExclusions(string[] args)
        {
            var exclusionsValue = GetCommandLineValue(args, "-xp");
            var fields = exclusionsValue?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return fields;
        }

        private static void GetExtendedLogging(string[] args)
        {
            var loggingValue = GetCommandLineValue(args, "-l");
            if (string.IsNullOrWhiteSpace(loggingValue))
            {
                loggingValue = ConfigurationManager.AppSettings["extendedLogging"];
            }

            if (string.IsNullOrWhiteSpace(loggingValue) || !bool.TryParse(loggingValue, out var logging))
            {
                logging = false;
            }

            Parameters.Settings.ExtendedLogging = logging;
        }

        private static string GetCommandLineValue(string[] args, string key)
        {
            var value = string.Empty;
            var keyIndex = Array.IndexOf(args, key);
            if (keyIndex >= 0 && args.Length > keyIndex + 1)
            {
                value = args[++keyIndex];
            }

            if (key == "-xp")
            {
                // add any additional values
                while (++keyIndex < args.Length && args[keyIndex].StartsWith(','))
                {
                    value += args[keyIndex];
                }
            }

            return value;
        }
    }
}
