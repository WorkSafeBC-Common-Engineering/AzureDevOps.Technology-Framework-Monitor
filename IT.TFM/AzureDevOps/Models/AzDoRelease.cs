namespace AzureDevOps.Models
{
    public class AzDoRelease
    {
        public AzDoReleaseDetails? Details { get; set; }
        public string? source { get; set; }
        public int revision { get; set; }
        public Createdby? createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public Modifiedby? modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
        public bool isDeleted { get; set; }
        public bool isDisabled { get; set; }
        public object? variableGroups { get; set; }
        public string? releaseNameFormat { get; set; }
        public string? comment { get; set; }
        public int id { get; set; }
        public string? name { get; set; }
        public string? path { get; set; }
        public object? projectReference { get; set; }
        public string? url { get; set; }
        public Links? _links { get; set; }
    }

    public class AzDoReleaseList
    {
        public int count { get; set; }
        public AzDoRelease[] value { get; set; } = [];
    }

    public class Links1
    {
        public Avatar? avatar { get; set; }
    }

    //public class Properties
    //{
    //}
}