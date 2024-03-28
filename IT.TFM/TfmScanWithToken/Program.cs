using Microsoft.VisualStudio.Services.ActivityStatistic;

using System;
using System.Configuration;
using System.Threading.Tasks;
using RepoFileScan = RepoScan.FileLocator;

namespace TfmScanWithToken
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var threadCount = GetTotalThreads(args);
            var forceDetails = GetForceDetails(args);

            var projectId = GetProjectId(args);
            var repositoryId = GetRepositoryId(args);

            await RepoScanAsync(projectId, repositoryId);
            await FileScanAsync(threadCount, forceDetails, projectId, repositoryId);
            PipelineScan(threadCount, repositoryId);
            await FileDetailsAsync(threadCount, forceDetails, projectId, repositoryId);
#if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
#endif
        }

        private static async Task RepoScanAsync(string projectId, string repositoryId)
        {
            Console.WriteLine($"Starting Repo Scan at: {DateTime.Now.ToLongTimeString()}");

            var scanner = new RepoFileScan.Scanner();
            await scanner.ScanAsync(projectId, repositoryId);

            Console.WriteLine($"Repo Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static async Task FileScanAsync(int threadCount, bool forceDetails, string projectId, string repositoryId)
        {
            Console.WriteLine($"Starting File Scan at: {DateTime.Now.ToLongTimeString()}");

            await RepoFileScan.FileProcessor.GetFiles(threadCount, forceDetails, projectId, repositoryId);

            Console.WriteLine($"File Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static void PipelineScan(int threadCount, string repositoryId)
        {
            Console.WriteLine($"Starting Pipeline Scan at: {DateTime.Now.ToLongTimeString()}");

            RepoFileScan.PipelineScanner.LinkPipelineToFile(threadCount, repositoryId);

            Console.WriteLine($"Pipeline Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static async Task FileDetailsAsync(int threadCount, bool forceDetails, string projectId, string repositoryId)
        {
            Console.WriteLine($"Starting File Details Scan at: {DateTime.Now.ToLongTimeString()}");

            await RepoFileScan.FileDetails.GetDetailsAsync(threadCount, forceDetails, projectId, repositoryId);

            Console.WriteLine($"File Details Scan complete at: {DateTime.Now.ToLongTimeString()}");
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

        private static string GetProjectId(string[] args)
        {
            return GetCommandLineValue(args, "-p");
        }

        private static string GetRepositoryId(string[] args)
        {
            return GetCommandLineValue(args, "-r");
        }

        private static string GetCommandLineValue(string[] args, string key)
        {
            var value = string.Empty;
            var keyIndex = Array.IndexOf(args, key);
            if (keyIndex >= 0 && args.Length > keyIndex + 1)
            {
                value = args[keyIndex + 1];
            }

            return value;
        }
    }
}
