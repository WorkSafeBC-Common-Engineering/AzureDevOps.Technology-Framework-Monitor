using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData
{
    public class PackageReference
    {
        public string PackageType { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }

        public string VersionComparator { get; set; }

        public string FrameworkVersion { get; set; }
    }
}
