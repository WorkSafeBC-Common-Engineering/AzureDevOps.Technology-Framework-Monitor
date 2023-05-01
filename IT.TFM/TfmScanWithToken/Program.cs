using System;
using System.Configuration;

namespace TfmScanWithToken
{
    class Program
    {
        private const string tokenVariable = "TFM_AdToken";

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"Please provide a single parameter with the AD Token to be used.");
            }

            else
            {
                // Set the token to be used in the environment, will be picked up when connecting to Azure DevOps
                Environment.SetEnvironmentVariable(tokenVariable, args[0]);

                var threadCount = GetTotalThreads();
                var forceDetails = GetForceDetails();

                RepoScan();
                FileScan(threadCount);
                FileDetails(threadCount, forceDetails);
            }
#if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
#endif
        }

        private static void RepoScan()
        {
            Console.WriteLine($"Starting Repo Scan at: {DateTime.Now.ToLongTimeString()}");

            var scanner = new RepoScan.FileLocator.Scanner();
            scanner.Scan();

            Console.WriteLine($"Repo Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static void FileScan(int threadCount)
        {
            Console.WriteLine($"Starting File Scan at: {DateTime.Now.ToLongTimeString()}");

            var fileLister = new RepoScan.FileLocator.FileProcessor();
            fileLister.GetFiles(threadCount);

            Console.WriteLine($"File Scan complete at: {DateTime.Now.ToLongTimeString()}");
        }

        private static void FileDetails(int threadCount, bool forceDetails)
        {
            Console.WriteLine($"Starting File Details Scan at: {DateTime.Now.ToLongTimeString()}");

            var fileDetailer = new RepoScan.FileLocator.FileDetails();
            fileDetailer.GetDetails(threadCount, forceDetails);

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
