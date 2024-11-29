namespace AzureDevOps.Models
{
    public class AzDoPipelineRunList
    {
        public int Count { get; set; }

        public AzDoPipelineRun[] Value { get; set; } = [];
    }

    public class AzDoPipelineRun
    {
        public Links? Links { get; set; }
        public Templateparameters? TemplateParameters { get; set; }
        public PipelineInfo? Pipeline { get; set; }
        public string? State { get; set; }
        public string? Result { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string? Url { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class Templateparameters
    {
    }

    public class PipelineInfo
    {
        public string? Url { get; set; }
        public int Id { get; set; }
        public int Revision { get; set; }
        public string? Name { get; set; }
        public string? Folder { get; set; }
    }
}
