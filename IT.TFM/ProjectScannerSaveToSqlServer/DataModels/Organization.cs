namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Organization
    {
        public Organization()
        {
            Projects = new HashSet<Project>();
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
