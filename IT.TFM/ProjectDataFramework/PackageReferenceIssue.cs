using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData
{
    public class PackageReferenceIssue
    {
        public int Id { get; set; }

        public string ScanType { get; set; }

        public string Framework { get; set; }

        public bool IsTopLevel { get; set; }

        public string PackageName { get; set; }

        public string RequestedVersion { get; set; }

        public string ResolvedVersion { get; set; }

        public string LatestVersion { get; set; }

        public string Severity { get; set; }

        public string AdvisoryUrl { get; set; }
    }
}
