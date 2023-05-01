namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FileReference
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        public int FileReferenceTypeId { get; set; }

        [StringLength(50)]
        public string PackageType { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string VersionComparator { get; set; }

        public string FrameworkVersion { get; set; }

        public string Path { get; set; }

        public virtual FileReferenceType FileReferenceType { get; set; }

        public virtual File File { get; set; }
    }
}
