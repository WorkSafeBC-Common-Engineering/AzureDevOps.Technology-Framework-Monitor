using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;
using Parser.Interfaces;
using ProjectData;
using ProjectData.Interfaces;
using TfmScanWithToken;

namespace PythonVersionUnitTests
{
    [CollectionDefinition("Console logging", DisableParallelization = true)]
    public class ConsoleLoggingCollection
    {
    }

    [Collection("Console logging")]
    public class LoggingTests
    {
        [Fact]
        public void CreateProvider_ResolvesLoggingServicesAndConsoleProviderWithoutInitialization()
        {
            using var provider = ServiceRegistration.CreateProvider();

            Assert.Single(provider.GetServices<ILoggerProvider>().OfType<ConsoleLoggerProvider>());
            Assert.Null(provider.GetService(typeof(Program)));
            Assert.IsType<RepoScan.FileLocator.Scanner>(provider.GetRequiredService<RepoScan.FileLocator.Scanner>());
            Assert.IsType<RepoScan.FileLocator.FileProcessor>(provider.GetRequiredService<RepoScan.FileLocator.FileProcessor>());
            Assert.IsType<RepoScan.FileLocator.PipelineScanner>(provider.GetRequiredService<RepoScan.FileLocator.PipelineScanner>());
            Assert.IsType<RepoScan.FileLocator.FileDetails>(provider.GetRequiredService<RepoScan.FileLocator.FileDetails>());
            Assert.IsType<RepoScan.FileLocator.RuntimeMetricsScan>(provider.GetRequiredService<RepoScan.FileLocator.RuntimeMetricsScan>());
            Assert.IsType<AzureDevOps.RestApi>(provider.GetRequiredService<AzureDevOps.IRestApi>());
            Assert.IsType<AzureDevOpsScannerFramework.Scanner>(provider.GetRequiredKeyedService<IScanner>("AzureDevOps"));
            Assert.IsType<NuGetScanner.Scanner>(provider.GetRequiredService<INuGetScanner>());
            Assert.IsType<RuntimeMetricsScanner.Scanner>(provider.GetRequiredService<IRuntimeMetricsScanner>());
            Assert.IsType<ProjectScannerSaveToSqlServer.Save>(provider.GetRequiredKeyedService<IStorageWriter>("SqlServerStorage"));
            Assert.IsType<ProjectScannerSaveToSqlServer.Read>(provider.GetRequiredKeyedService<IStorageReader>("SqlServerStorage"));
            Assert.IsType<VisualStudioMetricsScanner.MetricsScanner>(provider.GetRequiredKeyedService<IMetricsScanner>("VSSolution"));
            Assert.IsType<NuGetFileParser.NuspecParser>(provider.GetRequiredKeyedService<IFileParser>("Nuspec"));
            Assert.IsType<NuGetFileParser.ProjectPackagesParser>(provider.GetRequiredKeyedService<IFileParser>("NuGetPkgConfig"));
            Assert.IsType<EndOfLifeConfigFile.Checker>(provider.GetRequiredService<IEndOfLifeChecker>());
            Assert.IsType<ConfigurationFileData.ConfigurationSettings>(provider.GetRequiredService<ConfigurationFileData.ConfigurationSettings>());
            foreach (var key in new[] { "CSProject", "SqlProject", "VSXSolution", "VSConfig" })
            {
                Assert.IsAssignableFrom<IFileParser>(provider.GetRequiredKeyedService<IFileParser>(key));
            }
        }

        [Fact]
        public void CreateProvider_ConsoleOutputIncludesTimestampCategorySeverityAndException()
        {
            using var output = new StringWriter();
            var originalOutput = Console.Out;
            Console.SetOut(output);
            try
            {
                using var provider = ServiceRegistration.CreateProvider();
                var logger = provider.GetRequiredService<ILogger<FileItem>>();
                logger.LogInformation("Scan started for {Repository}", "example");
                logger.LogWarning("Scan warning for {Repository}", "example");
                var file = new FileItem(logger) { Path = "example.csproj" };
                file.AddProperty("Framework", "net11.0");
                file.AddProperty("Framework", "net10.0");
            }
            finally
            {
                Console.SetOut(originalOutput);
            }

            var text = output.ToString();
            Assert.Matches(@"\d{2}:\d{2}:\d{2} info: ProjectData.FileItem\[0\] Scan started for example", text);
            Assert.Contains("warn: ProjectData.FileItem[0] Scan warning for example", text);
            Assert.Contains("fail: ProjectData.FileItem[0] Unable to add property Framework to file example.csproj", text);
            Assert.Contains("System.ArgumentException", text);
        }

        [Fact]
        public void FileItem_WhenLoggerIsNull_RejectsIt()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new FileItem(null!));

            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public void AddProperty_WhenKeyAlreadyExists_LogsToInjectedLoggerAndPreservesOriginalValue()
        {
            var logger = new Mock<ILogger<FileItem>>();
            var file = new FileItem(logger.Object) { Path = "example.csproj" };
            file.AddProperty("Framework", "net11.0");

            file.AddProperty("Framework", "net10.0");

            Assert.Equal("net11.0", file.Properties["Framework"]);
            Assert.Single(file.Properties);
            logger.Verify(value => value.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => HasExpectedPropertyContext(state)),
                It.Is<Exception>(exception => exception is ArgumentException),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public void AddProperty_WithSeparateLoggers_DoesNotShareLoggingState()
        {
            var firstLogger = new Mock<ILogger<FileItem>>();
            var secondLogger = new Mock<ILogger<FileItem>>();
            var first = new FileItem(firstLogger.Object);
            var second = new FileItem(secondLogger.Object);
            first.AddProperty("Framework", "net11.0");
            second.AddProperty("Framework", "net11.0");

            first.AddProperty("Framework", "net10.0");

            firstLogger.Verify(value => value.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            secondLogger.VerifyNoOtherCalls();
            Assert.Equal("net11.0", second.Properties["Framework"]);
        }

        private static bool HasExpectedPropertyContext(object? state)
        {
            var properties = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(state).ToDictionary();
            Assert.Equal("Framework", properties["PropertyName"]);
            Assert.Equal("example.csproj", properties["FilePath"]);
            Assert.Equal("Unable to add property {PropertyName} to file {FilePath}", properties["{OriginalFormat}"]);
            return true;
        }
    }
}
