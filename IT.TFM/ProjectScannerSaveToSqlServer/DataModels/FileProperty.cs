namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.ComponentModel.DataAnnotations;

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
