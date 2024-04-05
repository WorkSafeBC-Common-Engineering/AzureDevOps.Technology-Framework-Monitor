namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Organization
    {
        public Organization()
        {
            Projects = [];
        }

        public int Id { get; set; }

        public int ScannerTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Uri { get; set; }

        public virtual ScannerType ScannerType { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
