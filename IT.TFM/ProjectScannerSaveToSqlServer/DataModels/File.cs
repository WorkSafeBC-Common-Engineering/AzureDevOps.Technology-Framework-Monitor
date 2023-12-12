namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class File
    {
        public File()
        {
            FileProperties = new HashSet<FileProperty>();
            FileReferences = new HashSet<FileReference>();
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

        public virtual FileType FileType { get; set; }

        public virtual Repository Repository { get; set; }
    }
}
