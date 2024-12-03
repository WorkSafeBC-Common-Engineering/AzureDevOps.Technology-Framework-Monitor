namespace AzureDevOps.Models
{
    public class AzDoRelease
    {
        public AzDoReleaseDetails? Details { get; set; }
        public string? Source { get; set; }
        public int Revision { get; set; }
        public UserInfo? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public UserInfo? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
        public object? VariableGroups { get; set; }
        public string? ReleaseNameFormat { get; set; }
        public string? Comment { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public ProjectReference? ProjectReference { get; set; }
        public string? Url { get; set; }
        public Links? Links { get; set; }

        // the following are added seperately with information gleaned from another call
        public DateTime? LastRunStart { get; set; }
        public DateTime? LastRunEnd { get; set; }
        public string? State { get; set; }
        public string? Result { get; set; }
        public string? LastRunUrl { get; set; }
    }

    public class AzDoReleaseList
    {
        public int Count { get; set; }
        public AzDoRelease[] Value { get; set; } = [];
    }
}