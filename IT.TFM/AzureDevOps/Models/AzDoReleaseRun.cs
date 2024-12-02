using System.Text.Json.Serialization;

namespace AzureDevOps.Models
{
    public class AzDoReleaseRunsList
    {
        public int Count { get; set; }

        public AzDoReleaseRun[] Value { get; set; } = [];
    }

    public class AzDoReleaseRun
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public UserInfo? ModifiedBy { get; set; }
        public UserInfo? CreatedBy { get; set; }
        public UserInfo? CreatedFor { get; set; }
        public Variables? Variables { get; set; }
        public object[] VariableGroups { get; set; } = [];
        public ReleaseDefinition? ReleaseDefinition { get; set; }
        public int ReleaseDefinitionRevision { get; set; }
        public string? Description { get; set; }
        public string? Reason { get; set; }
        public string? ReleaseNameFormat { get; set; }
        public bool? KeepForever { get; set; }
        public int DefinitionSnapshotRevision { get; set; }
        public string? LogsContainerUrl { get; set; }
        public string? Url { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
        public object[] Tags { get; set; } = [];
        public object? TriggeringArtifactAlias { get; set; }
        public ProjectReference? ProjectReference { get; set; }
        public Properties? Properties { get; set; }
    }

    public class Release
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public object[] Artifacts { get; set; } = [];
        public string? WebAccessUri { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
    }

    public class ProjectReference
    {
        public string? Id { get; set; }
        public object? Name { get; set; }
    }

    public class ReleaseEnvironment
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
    }

    public class RequestedBy
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Descriptor { get; set; }
    }

    public class RequestedFor
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Descriptor { get; set; }
    }

    public class LastModifiedBy
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Descriptor { get; set; }
    }

    public class PreDeployApproval
    {
        public int Id { get; set; }
        public int Revision { get; set; }
        public Approver? Approver { get; set; }
        public string? ApprovalType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public bool IsAutomated { get; set; }
        public bool IsNotificationOn { get; set; }
        public int TrialNumber { get; set; }
        public int Attempt { get; set; }
        public int Rank { get; set; }
        public Release? Release { get; set; }
        public ReleaseDefinition? ReleaseDefinition { get; set; }
        public ReleaseEnvironment? ReleaseEnvironment { get; set; }
        public string? Url { get; set; }
        public ApprovedBy? ApprovedBy { get; set; }
    }

    public class ApprovedBy
    {
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
        public string? Id { get; set; }
        public string? UniqueName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Descriptor { get; set; }
    }

    public class PostDeployApproval
    {
        public int Id { get; set; }
        public int Revision { get; set; }
        public Approver? Approver { get; set; }
        public string? Inactive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public bool IsAutomated { get; set; }
        public bool IsNotificationOn { get; set; }
        public int TrialNumber { get; set; }
        public int Attempt { get; set; }
        public int Rank { get; set; }
        public Release? Release { get; set; }
        public ReleaseDefinition? ReleaseDefinition { get; set; }
        public ReleaseEnvironment? ReleaseEnvironment { get; set; }
        public string? Url { get; set; }
    }
}
