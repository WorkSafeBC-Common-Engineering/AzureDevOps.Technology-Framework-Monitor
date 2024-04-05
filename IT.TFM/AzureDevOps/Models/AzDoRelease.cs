namespace AzureDevOps.Models
{
    public class AzDoRelease
    {
        public AzDoReleaseDetails? Details { get; set; }
        public string? Source { get; set; }
        public int Revision { get; set; }
        public Createdby? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Modifiedby? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
        public object? VariableGroups { get; set; }
        public string? ReleaseNameFormat { get; set; }
        public string? Comment { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public object? ProjectReference { get; set; }
        public string? Url { get; set; }
        public Links? Links { get; set; }
    }

    public class AzDoReleaseList
    {
        public int Count { get; set; }
        public AzDoRelease[] Value { get; set; } = [];
    }

    public class Links1
    {
        public Avatar? Avatar { get; set; }
    }
}