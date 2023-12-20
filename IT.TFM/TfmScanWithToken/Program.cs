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
            var threadCount = GetTotalThreads();
            var forceDetails = GetForceDetails();

            await RepoScanAsync();
            await FileScanAsync(threadCount, forceDetails);
            await FileDetailsAsync(threadCount, forceDetails);
#if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
#endif
        }

        private static async Task RepoScanAsync()
        {
            Console.WriteLine($"Starting Repo Scan at: {DateTime.Now.ToLongTimeString()}");

            var scanner = new RepoFileScan.Scanner();
            await scanner.ScanAsync();

            Console.WriteLine($"Repo Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static async Task FileScanAsync(int threadCount, bool forceDetails)
        {
            Console.WriteLine($"Starting File Scan at: {DateTime.Now.ToLongTimeString()}");

            await RepoFileScan.FileProcessor.GetFiles(threadCount, forceDetails);

            Console.WriteLine($"File Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static async Task FileDetailsAsync(int threadCount, bool forceDetails)
        {
            Console.WriteLine($"Starting File Details Scan at: {DateTime.Now.ToLongTimeString()}");

            await RepoFileScan.FileDetails.GetDetailsAsync(threadCount, forceDetails);

            Console.WriteLine($"File Details Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static int GetTotalThreads()
        {
            var threadValue = ConfigurationManager.AppSettings["processingThreads"];
            if (string.IsNullOrWhiteSpace(threadValue) || !int.TryParse(threadValue, out var threadCount))
            {
                threadCount = 1;
            }

            return threadCount;
        }

        private static bool GetForceDetails()
        {
            var forceDetailsValue = ConfigurationManager.AppSettings["forceDetails"];
            if (string.IsNullOrWhiteSpace(forceDetailsValue) || !bool.TryParse(forceDetailsValue, out var forceDetails))
            {
                forceDetails = false;
            }

            return forceDetails;
        }
    }
}
