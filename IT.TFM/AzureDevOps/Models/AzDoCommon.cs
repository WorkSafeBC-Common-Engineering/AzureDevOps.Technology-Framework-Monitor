using Newtonsoft.Json;

namespace AzureDevOps.Models
{
    public class Self
    {
        public string? Href { get; set; }
    }

    public class Web
    {
        public string? Href { get; set; }
    }

    public class PipelineWeb
    {
        public string? Href { get; set; }
    }

    public class Pipeline
    {
        public string? Href { get; set; }
    }

    public class Links
    {
        public Self? Self { get; set; }
        public Web? Web { get; set; }
        public Avatar? Avatar { get; set; }

        public PipelineWeb? PipelineWeb { get; set; }
        public Pipeline? Pipeline { get; set; }
    }

    public class Avatar
    {
        public string? Href { get; set; }
    }

    public class UserInfo
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        public Links? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Descriptor { get; set; }
        public bool? Inactive { get; set; }
    }

    public class SystemDebug
    {
        public string? Value { get; set; }
        public bool? AllowOverride { get; set; }
    }

    public class Definition
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Variables
    {
        public BuildConfiguration? BuildConfiguration { get; set; }
        public BuildPlatform? BuildPlatform { get; set; }
        [JsonProperty("Portfolio.ProductName")]
        public PortfolioProductName? PortfolioProductName { get; set; }
        public SystemDebug? SystemDebug { get; set; }
        public Approotfolder? AppRootFolder { get; set; }
        public Productfamilyname? ProductFamilyName { get; set; }
        public SystemDebug? Systemdebug { get; set; }
        public Temprootfolder? TempRootFolder { get; set; }
        public Webservicename? WebServiceName { get; set; }
        public Websitenamelayer7? WebsiteNameLayer7 { get; set; }
        public Apppoolidentityusername? AppPoolIdentityUsername { get; set; }

    }

    public class ReleaseDefinition
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public ProjectReference? ProjectReference { get; set; }
        public string? Url { get; set; }
        public Links? _links { get; set; }
    }

    public class Condition
    {
        public string? Name { get; set; }
        public string? ConditionType { get; set; }
        public string? Value { get; set; }
        public bool? Result { get; set; }
    }

    public class Approver
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        public Links? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsContainer { get; set; }
        public bool? Inactive { get; set; }
        public string? Descriptor { get; set; }
    }
}
