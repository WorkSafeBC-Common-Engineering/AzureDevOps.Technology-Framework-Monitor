using System;

namespace ProjectData
{
    public class Pipeline
    {
        public int Id { get; set; } = 0;

        public string RepositoryId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Folder { get; set; } = string.Empty;

        public int Revision { get; set; } = 0;

        public string Url { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string PipelineType { get; set; } = string.Empty;

        public string Path {  get; set; } = string.Empty;
    }
}
