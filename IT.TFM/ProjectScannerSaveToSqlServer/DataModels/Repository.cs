namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Repository
    {
        public Repository()
        {
            Files = new HashSet<File>();
            Pipelines = new HashSet<Pipeline>();
        }

        public int Id { get; set; }

        public int ProjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string RepositoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Portfolio { get; set; }

        public string ApplicationProjectName { get; set; }

        public string ComponentName { get; set; }

        public string DefaultBranch { get; set; }

        public bool IsFork { get; set; }

        public long Size { get; set; }

        [Required]
        public string Url { get; set; }

        public string RemoteUrl { get; set; }

        public string WebUrl { get; set; }

        public bool Deleted { get; set; }

        public bool NoScan { get; set; }

        [StringLength(50)]
        public string LastCommitId { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public virtual ICollection<Pipeline> Pipelines { get; set; }

        public virtual Project Project { get; set; }
    }
}
