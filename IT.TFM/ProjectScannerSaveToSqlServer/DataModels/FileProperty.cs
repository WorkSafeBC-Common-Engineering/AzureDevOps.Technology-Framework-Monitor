namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileProperty
    {
        public int Id { get; set; }

        public int PropertyTypeId { get; set; }

        public int FileId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Value { get; set; }
        
        public virtual FilePropertyType FilePropertyType { get; set; }

        public virtual File File { get; set; }
    }
}
