namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class File
    {
        public File()
        {
            FileProperties = [];
            FileReferences = [];
            PackageIssues = [];
        }

        public int Id { get; set; }

        public int RepositoryId { get; set; }

        public int FileTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string FileId { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Url { get; set; }

        [StringLength(50)]
        public string CommitId { get; set; }

        public virtual ICollection<FileProperty> FileProperties { get; set; }

        public virtual ICollection<FileReference> FileReferences { get; set; }

        public virtual ICollection<PackageIssue> PackageIssues { get; set; }

        public virtual FileType FileType { get; set; }

        public virtual Repository Repository { get; set; }

        //public virtual Pipeline Pipeline { get; set; }
    }
}
