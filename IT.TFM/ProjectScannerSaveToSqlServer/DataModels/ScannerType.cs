namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ScannerType
    {
        public ScannerType()
        {
            Organizations = [];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Value { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; }
    }
}
