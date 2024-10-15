using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class NuGetPackage
    {
        public NuGetPackage()
        {
            NuGetTargetFrameworks = [];
        }

        public int Id { get; set; }

        public int? RepositoryId { get; set; }

        public int NuGetFeedId { get; set; }

        public string Name { get; set; }

        [StringLength(50)]
        public string Version { get; set; }

        public string Description { get; set; }

        public string Authors { get; set; }

        public DateTime Published { get; set; }

        public string ProjectUrl { get; set; }

        public string Tags { get; set; }
        
        public virtual Repository Repository { get; set; }

        public virtual NuGetFeed NuGetFeed { get; set; }

        public virtual ICollection<NuGetTargetFramework> NuGetTargetFrameworks { get; set; }
    }
}
