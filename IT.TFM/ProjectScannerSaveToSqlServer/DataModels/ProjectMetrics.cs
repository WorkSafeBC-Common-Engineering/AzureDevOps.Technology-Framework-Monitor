using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public partial class ProjectMetrics
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        public int MaintainabilityIndex { get; set; }

        public int CyclomaticComplexity { get; set; }

        public int DepthOfInheritance { get; set; }

        public int SourceLines { get; set; }

        public int ExecutableLines { get; set; }

        public virtual File File { get; set; }
    }
}
