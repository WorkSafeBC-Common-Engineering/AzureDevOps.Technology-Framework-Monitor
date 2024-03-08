using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Models
{
    public class PipelineRepository
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }

    public class Links
    {
        public Self Self { get; set; }
        public Web Web { get; set; }
    }

    public class Self
    {
        public string Href { get; set; }
    }

    public class Web
    {
        public string Href { get; set; }
    }

    public class Configuration
    {
        public DesignerJson DesignerJson { get; set; }
        public string Path { get; set; }
        public PipelineRepository Repository { get; set; }
        public string Type { get; set; }
    }

    public class DesignerJson
    {
        public List<Option> Options { get; set; }
        public Variables Variables { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<string> Tags { get; set; }
        public Links Links { get; set; }
        public string BuildNumberFormat { get; set; }
        public string Comment { get; set; }
        public string JobAuthorizationScope { get; set; }
        public int JobTimeoutInMinutes { get; set; }
        public int JobCancelTimeoutInMinutes { get; set; }
        public Process Process { get; set; }
        public PipelineRepository Repository { get; set; }
        public string Type { get; set; }
        
    }

    public class Option
    {
        public bool Enabled { get; set; }
        public Definition Definition { get; set; }
        public OptionInputs Inputs { get; set; }
    }

    public class Definition
    {
        public string Id { get; set; }
    }

    public class OptionInputs
    {
        public string BranchFilters { get; set; }
        public string AdditionalFields { get; set; }
        public string WorkItemType { get; set; }
        public string AssignToRequestor { get; set; }
    }

    public class Variables
    {
        public BuildConfiguration BuildConfiguration { get; set; }
        public BuildPlatform BuildPlatform { get; set; }
        public PortfolioProductName PortfolioProductName { get; set; }
        public SystemDebug SystemDebug { get; set; }
    }

    public class BuildConfiguration
    {
        public string Value { get; set; }
        public bool AllowOverride { get; set; }
    }

    public class BuildPlatform
    {
        public string Value { get; set; }
        public bool AllowOverride { get; set; }
    }

    public class PortfolioProductName
    {
        public string Value { get; set; }
    }

    public class SystemDebug
    {
        public string Value { get; set; }
        public bool AllowOverride { get; set; }
    }

    public class Process
    {
        public List<Phase> Phases { get; set; }
    }

    public class Phase
    {
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        public StepEnvironment Environment { get; set; }
        public bool Enabled { get; set; }
        public bool ContinueOnError { get; set; }
        public bool AlwaysRun { get; set; }
        public string DisplayName { get; set; }
        public int TimeoutInMinutes { get; set; }
        public int RetryCountOnTaskFailure { get; set; }
        public string Condition { get; set; }
        public PipelineTask Task { get; set; }
        public StepInputs Inputs { get; set; }
    }

    public class StepEnvironment
    {
    }

    public class PipelineTask
    {
        public string Id { get; set; }
        public string VersionSpec { get; set; }
        public string DefinitionType { get; set; }
    }

    public class StepInputs
    {
        public string PackageType { get; set; }
        public string UseGlobalJson { get; set; }
        public string WorkingDirectory { get; set; }
        public string Version { get; set; }
        public string VsVersion { get; set; }
        public string IncludePreviewVersions { get; set; }
        public string InstallationPath { get; set; }
        public string PerformMultiLevelLookup { get; set; }
        public string Command { get; set; }
        public string PublishWebProjects { get; set; }
        public string Projects { get; set; }
        public string Custom { get; set; }
        public string Arguments { get; set; }
        public string RestoreArguments { get; set; }
        public string PublishTestResults { get; set; }
        public string TestRunTitle { get; set; }
        public string ZipAfterPublish { get; set; }
        public string ModifyOutputPath { get; set; }
        public string SelectOrConfig { get; set; }
        public string FeedRestore { get; set; }
        public string IncludeNuGetOrg { get; set; }
        public string NugetConfigPath { get; set; }
        public string ExternalEndpoints { get; set; }
        public string NoCache { get; set; }
        public string PackagesDirectory { get; set; }
        public string VerbosityRestore { get; set; }
        public string SearchPatternPush { get; set; }
        public string NuGetFeedType { get; set; }
        public string FeedPublish { get; set; }
        public string PublishPackageMetadata { get; set; }
        public string ExternalEndpoint { get; set; }
        public string SearchPatternPack { get; set; }
        public string ConfigurationToPack { get; set; }
        public string OutputDir { get; set; }
        public string Nobuild { get; set; }
        public string Includesymbols { get; set; }
        public string Includesource { get; set; }
        public string VersioningScheme { get; set; }
        public string VersionEnvVar { get; set; }
        public string RequestedMajorVersion { get; set; }
        public string RequestedMinorVersion { get; set; }
        public string RequestedPatchVersion { get; set; }
        public string BuildProperties { get; set; }
        public string VerbosityPack { get; set; }
        public string RefName { get; set; }
        public string SonarQube { get; set; }
        public string ScannerMode { get; set; }
        public string ConfigMode { get; set; }
        public string ConfigFile { get; set; }
        public string CliProjectKey { get; set; }
        public string ProjectKey { get; set; }
        public string CliProjectName { get; set; }
        public string ProjectName { get; set; }
        public string CliProjectVersion { get; set; }
        public string ProjectVersion { get; set; }
        public string CliSources { get; set; }
        public string ExtraProperties { get; set; }
        public string ConnectedServiceNameSelector { get; set; }
        public string ConnectedServiceName { get; set; }
        public string ConnectedServiceNameARM { get; set; }
        public string ScriptType { get; set; }
        public string ScriptPath { get; set; }
        public string Inline { get; set; }
        public string ScriptArguments { get; set; }
        public string ErrorActionPreference { get; set; }
        public string FailOnStandardError { get; set; }
        public string TargetAzurePs { get; set; }
        public string CustomTargetAzurePs { get; set; }
    }

    public class AzDoPipelineDetails
    {
        public Links Links { get; set; }
        public Configuration Configuration { get; set; }
        public string Url { get; set; }
        public int Id { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
    }
}
