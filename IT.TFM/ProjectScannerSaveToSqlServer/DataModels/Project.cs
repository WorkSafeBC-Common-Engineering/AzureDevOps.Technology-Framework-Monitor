namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            Repositories = new HashSet<Repository>();
        }

        public int Id { get; set; }

        public int OrganizationId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectId { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        public string Abbreviation { get; set; }

        public string Description { get; set; }

        public DateTime? LastUpdate { get; set; }

        public long Revision { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [Required]
        public string Url { get; set; }

        [StringLength(50)]
        public string Visibility { get; set; }

        public bool Deleted { get; set; }

        public virtual Organization Organization { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
