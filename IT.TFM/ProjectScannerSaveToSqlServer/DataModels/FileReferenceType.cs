namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileReferenceType
    {
        public FileReferenceType()
        {
            FileReferences = [];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Value { get; set; }

        public virtual ICollection<FileReference> FileReferences { get; set; }
    }
}
