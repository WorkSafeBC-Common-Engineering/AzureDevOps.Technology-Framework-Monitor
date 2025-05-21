using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData
{
    public class Metrics
    {
        public int MaintainabilityIndex { get; set; }

        public int CyclomaticComplexity { get; set; }

        public int ClassCoupling { get; set; }

        public int DepthOfInheritance { get; set; }

        public int SourceLines { get; set; }

        public int ExecutableLines { get; set; }

        public byte UnitTestCodeCoverage { get; set; }
    }
}
