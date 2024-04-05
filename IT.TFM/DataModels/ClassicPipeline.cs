namespace RepoScan.DataModels
{
    public class ClassicPipeline
    {
        public int PipelineId { get; set; }

        public string RepositoryId { get; set; }

        public string Name { get; set; }

        public string Folder { get; set; }

        public string Type { get; set; }

        public bool IsBuildPipeline { get; set; } = true;
    }
}
