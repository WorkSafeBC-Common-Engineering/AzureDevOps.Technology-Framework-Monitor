using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData
{
    public class NuGetPackage
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public bool DataLoaded { get; set; } = false;

        public string Description { get; set; }

        public string Authors { get; set; }

        public DateTime? Published { get; set; }

        public Uri ProjectUrl { get; set; }

        public string Tags { get; set; }

        public NuGetFeed Feed { get; set; }

        public string Project { get; set; }

        public string Repository { get; set; }

        public NuGetTarget[] Targets { get; set; }
    }
}
    