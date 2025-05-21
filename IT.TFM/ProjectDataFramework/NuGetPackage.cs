using System;

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

        public string RepositoryId { get; set; }

        public NuGetTarget[] Targets { get; set; }
    }
}
    