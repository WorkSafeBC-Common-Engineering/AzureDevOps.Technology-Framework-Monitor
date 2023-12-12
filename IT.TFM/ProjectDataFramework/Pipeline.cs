using System;

namespace ProjectData
{
    public class Pipeline
    {
        public int Id { get; set; } = 0;

        public string RepositoryName { get; set; } = string.Empty;

        public string RepositoryId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Folder { get; set; } = string.Empty;

        public int Revision { get; set; } = 0;

        public string Url { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string PipelineType { get; set; } = string.Empty;

        public string QueueStatus { get; set; } = string.Empty;

        public string Quality { get; set; } = string.Empty;

        public string CreatedBy { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
