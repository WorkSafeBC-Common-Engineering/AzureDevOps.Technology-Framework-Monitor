namespace RepoScan.DataModels
{
    public class YamlPipeline
    {
        public int PipelineId { get; set; }

        public string RepositoryId { get; set; }

        public string FileId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            return $"Id: {PipelineId}, RepositoryId: {RepositoryId}, FileId: {FileId}, Name: {Name}, Path: {Path}";
        }
    }
}
