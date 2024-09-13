using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class PackageIssue
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string ScanType { get; set; }

        [Required]
        public int FileId { get; set; }

        [StringLength(50)]
        public string Framework { get; set; }

        public bool IsTopLevel { get; set; }

        [Required]
        public string PackageName { get; set; }

        [StringLength(50)]
        public string? RequestedVersion { get; set; }

        [StringLength(50)]
        public string? ResolvedVersion { get; set; }

        [StringLength(50)]
        public string? LatestVersion { get; set; }

        [StringLength(50)]
        public string? Severity { get; set; }

        public string? AdvisoryUrl { get; set; }

        public virtual File File { get; set; }
    }
}
