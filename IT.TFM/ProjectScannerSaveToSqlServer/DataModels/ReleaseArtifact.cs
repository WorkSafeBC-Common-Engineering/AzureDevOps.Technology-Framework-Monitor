namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class ReleaseArtifact
    {
        public int Id { get; set; }

        public int PipelineId { get; set; }

        public string SourceId { get; set; }

        public string Type { get; set; }

        public string Alias { get; set; }

        public string Url { get; set; }

        public string DefaultVersionType { get; set; }

        public string DefinitionId { get; set; }

        public string DefinitionName { get; set; }

        public string Project { get; set; }

        public string ProjectId { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsRetained { get; set; }

        public virtual Pipeline Pipeline { get; set; }
    }
}
