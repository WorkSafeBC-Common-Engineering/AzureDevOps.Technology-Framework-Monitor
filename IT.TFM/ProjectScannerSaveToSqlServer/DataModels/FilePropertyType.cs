namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FilePropertyType
    {
        public FilePropertyType()
        {
            FileProperties = new HashSet<FileProperty>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Value { get; set; }

        public virtual ICollection<FileProperty> FileProperties { get; set; }
    }
}

