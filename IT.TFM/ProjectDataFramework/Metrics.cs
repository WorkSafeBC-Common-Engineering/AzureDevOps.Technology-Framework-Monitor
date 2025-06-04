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

        public int? LastRunTotalWarnings { get; set; }

        public int? LastRunTotalErrors { get; set; }
    }
}
