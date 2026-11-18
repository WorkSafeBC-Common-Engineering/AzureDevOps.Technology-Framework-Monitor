using Microsoft.Extensions.DependencyInjection;

using Parser.Interfaces;
using ProjectData.Interfaces;
using RepoScan.DataModels;
using RepoScan.Storage.SqlServer;
using VisualStudioFileParser;

namespace TfmScanWithToken
{
    internal static class ServiceRegistration
    {
        internal static ServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();

            services.AddVisualStudioFileParsers();
            services.AddRepoScanSqlServerStorage();

            services.AddTransient<IFilterConfiguration, FilterConfigurationFromConfigFile.Configuration>();
            services.AddKeyedTransient<IFilter, ContentFilter.Filter>("Content");
            services.AddKeyedTransient<IContentParser, OpenSecretsDetection.SecretsParser>("OpenSecrets");
            services.AddTransient<IEndOfLifeChecker, EndOfLifeConfigFile.Checker>();

            services.AddTransient<INuGetScanner, NuGetScanner.Scanner>();
            services.AddTransient<IRuntimeMetricsScanner, RuntimeMetricsScanner.Scanner>();
            services.AddKeyedTransient<IScanner, AzureDevOpsScannerFramework.Scanner>("AzureDevOps");

            services.AddKeyedTransient<IStorageWriter, ProjectScannerSaveToSqlServer.Save>("SqlServerStorage");
            services.AddKeyedTransient<IStorageReader, ProjectScannerSaveToSqlServer.Read>("SqlServerStorage");

            foreach (var fileType in new[]
            {
                "CSProject", "VBProject", "VB6Project", "SqlProject", "VSConfig",
                "NuGetPkgConfig", "Nuspec", "NpmPackage", "YamlPipeline", "Dll",
                "Nvmrc", "PythonVersion", "PythonProjectToml", "PythonSetupPy",
                "PythonSetupCfg", "PythonDockerfile"
            })
            {
                services.AddKeyedTransient<IMetricsScanner, DefaultMetricsScanner.MetricsScanner>(fileType);
            }

            services.AddKeyedTransient<IMetricsScanner, VisualStudioMetricsScanner.MetricsScanner>("VSSolution");
            services.AddKeyedTransient<IMetricsScanner, VisualStudioMetricsScanner.MetricsScanner>("VSXSolution");

            services.AddKeyedTransient<IFileParser, NuGetFileParser.ProjectPackagesParser>("NuGetPkgConfig");
            services.AddKeyedTransient<IFileParser, NuGetFileParser.NuspecParser>("Nuspec");
            services.AddKeyedTransient<IFileParser, NpmFileParser.PackageParser>("NpmPackage");
            services.AddKeyedTransient<IFileParser, NpmFileParser.NvmrcParser>("Nvmrc");
            services.AddKeyedTransient<IFileParser, YamlFileParser.PipelineParser>("YamlPipeline");
            services.AddKeyedTransient<IFileParser, PythonFileParser.PythonVersionParser>("PythonVersion");
            services.AddKeyedTransient<IFileParser, PythonFileParser.PythonProjectTomlParser>("PythonProjectToml");
            services.AddKeyedTransient<IFileParser, PythonFileParser.PythonSetupPyParser>("PythonSetupPy");
            services.AddKeyedTransient<IFileParser, PythonFileParser.PythonSetupCfgParser>("PythonSetupCfg");
            services.AddKeyedTransient<IFileParser, PythonFileParser.PythonDockerfileParser>("PythonDockerfile");

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }
    }
}
