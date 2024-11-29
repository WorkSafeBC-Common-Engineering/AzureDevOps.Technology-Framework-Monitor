using System;

namespace ProjectData
{
    public class Pipeline
    {
        public const string pipelineTypeYaml = "yaml";
        public const string pipelineTypeClassic = "designerJson";
        public const string pipelineTypeRelease = "release";
        public const string pipelineRelease = "release";

        public int Id { get; set; } = 0;

        public string ProjectId { get; set; } = string.Empty;

        public string RepositoryId { get; set; } = string.Empty;

        public string FileId { get; set; } = null;

        public string Name { get; set; } = string.Empty;

        public string Folder { get; set; } = string.Empty;

        public int Revision { get; set; } = 0;

        public string Url { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string PipelineType { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string YamlType { get; set; }

        public string Portfolio { get;set; }

        public string Product { get; set; }

        public string State { get; set; } = string.Empty;

        public string Result {  get; set; } = string.Empty;

        public DateTime? LastRunStart {  get; set; }

        public DateTime? LastRunEnd { get; set; }

        public string LastRunUrl { get; set; } = string.Empty;
    }
}
