using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Models
{
    //public class Self
    //{
    //    public string href { get; set; }
    //}

    //public class Web
    //{
    //    public string href { get; set; }
    //}

    //public class Links
    //{
    //    public Self self { get; set; }
    //    public Web web { get; set; }
    //}

    public class PipelineRepository
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    //public class Configuration
    //{
    //    public string path { get; set; }
    //    public PipelineRepository repository { get; set; }
    //    public string type { get; set; }
    //}

    //public class DesignerJsonX
    //{
    //    public Links _links { get; set; }
    //    public Configuration configuration { get; set; }
    //}

    public class Links
    {
        public Self self { get; set; }
        public Web web { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class Configuration
    {
        public DesignerJson designerJson { get; set; }
        public string path { get; set; }
        public PipelineRepository repository { get; set; }
        public string type { get; set; }
    }

    //public class Configuration
    //{
    //    public DesignerJson designerJson { get; set; }
    //}

    public class DesignerJson
    {
        public List<Option> options { get; set; }
        public Variables variables { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public List<string> tags { get; set; }
        public Links _links { get; set; }
        public string buildNumberFormat { get; set; }
        public string comment { get; set; }
        public string jobAuthorizationScope { get; set; }
        public int jobTimeoutInMinutes { get; set; }
        public int jobCancelTimeoutInMinutes { get; set; }
        public Process process { get; set; }
        public PipelineRepository repository { get; set; }
        public string type { get; set; }
        
    }

    public class Option
    {
        public bool enabled { get; set; }
        public Definition definition { get; set; }
        public OptionInputs inputs { get; set; }
    }

    public class Definition
    {
        public string id { get; set; }
    }

    public class OptionInputs
    {
        public string branchFilters { get; set; }
        public string additionalFields { get; set; }
        public string workItemType { get; set; }
        public string assignToRequestor { get; set; }
    }

    public class Variables
    {
        public BuildConfiguration BuildConfiguration { get; set; }
        public BuildPlatform BuildPlatform { get; set; }
        public PortfolioProductName PortfolioProductName { get; set; }
        public SystemDebug systemDebug { get; set; }
    }

    public class BuildConfiguration
    {
        public string value { get; set; }
        public bool allowOverride { get; set; }
    }

    public class BuildPlatform
    {
        public string value { get; set; }
        public bool allowOverride { get; set; }
    }

    public class PortfolioProductName
    {
        public string value { get; set; }
    }

    public class SystemDebug
    {
        public string value { get; set; }
        public bool allowOverride { get; set; }
    }

    public class Process
    {
        public List<Phase> phases { get; set; }
    }

    public class Phase
    {
        public List<Step> steps { get; set; }
    }

    public class Step
    {
        public StepEnvironment environment { get; set; }
        public bool enabled { get; set; }
        public bool continueOnError { get; set; }
        public bool alwaysRun { get; set; }
        public string displayName { get; set; }
        public int timeoutInMinutes { get; set; }
        public int retryCountOnTaskFailure { get; set; }
        public string condition { get; set; }
        public PipelineTask task { get; set; }
        public StepInputs inputs { get; set; }
    }

    public class StepEnvironment
    {
    }

    public class PipelineTask
    {
        public string id { get; set; }
        public string versionSpec { get; set; }
        public string definitionType { get; set; }
    }

    public class StepInputs
    {
        public string packageType { get; set; }
        public string useGlobalJson { get; set; }
        public string workingDirectory { get; set; }
        public string version { get; set; }
        public string vsVersion { get; set; }
        public string includePreviewVersions { get; set; }
        public string installationPath { get; set; }
        public string performMultiLevelLookup { get; set; }
        public string command { get; set; }
        public string publishWebProjects { get; set; }
        public string projects { get; set; }
        public string custom { get; set; }
        public string arguments { get; set; }
        public string restoreArguments { get; set; }
        public string publishTestResults { get; set; }
        public string testRunTitle { get; set; }
        public string zipAfterPublish { get; set; }
        public string modifyOutputPath { get; set; }
        public string selectOrConfig { get; set; }
        public string feedRestore { get; set; }
        public string includeNuGetOrg { get; set; }
        public string nugetConfigPath { get; set; }
        public string externalEndpoints { get; set; }
        public string noCache { get; set; }
        public string packagesDirectory { get; set; }
        public string verbosityRestore { get; set; }
        public string searchPatternPush { get; set; }
        public string nuGetFeedType { get; set; }
        public string feedPublish { get; set; }
        public string publishPackageMetadata { get; set; }
        public string externalEndpoint { get; set; }
        public string searchPatternPack { get; set; }
        public string configurationToPack { get; set; }
        public string outputDir { get; set; }
        public string nobuild { get; set; }
        public string includesymbols { get; set; }
        public string includesource { get; set; }
        public string versioningScheme { get; set; }
        public string versionEnvVar { get; set; }
        public string requestedMajorVersion { get; set; }
        public string requestedMinorVersion { get; set; }
        public string requestedPatchVersion { get; set; }
        public string buildProperties { get; set; }
        public string verbosityPack { get; set; }
        public string refName { get; set; }
        public string SonarQube { get; set; }
        public string scannerMode { get; set; }
        public string configMode { get; set; }
        public string configFile { get; set; }
        public string cliProjectKey { get; set; }
        public string projectKey { get; set; }
        public string cliProjectName { get; set; }
        public string projectName { get; set; }
        public string cliProjectVersion { get; set; }
        public string projectVersion { get; set; }
        public string cliSources { get; set; }
        public string extraProperties { get; set; }
        public string ConnectedServiceNameSelector { get; set; }
        public string ConnectedServiceName { get; set; }
        public string ConnectedServiceNameARM { get; set; }
        public string ScriptType { get; set; }
        public string ScriptPath { get; set; }
        public string Inline { get; set; }
        public string ScriptArguments { get; set; }
        public string errorActionPreference { get; set; }
        public string FailOnStandardError { get; set; }
        public string TargetAzurePs { get; set; }
        public string CustomTargetAzurePs { get; set; }
    }

    public class AzDoPipelineDetails
    {
        public Links _links { get; set; }
        public Configuration configuration { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public int revision { get; set; }
        public string name { get; set; }
        public string folder { get; set; }
    }
}
