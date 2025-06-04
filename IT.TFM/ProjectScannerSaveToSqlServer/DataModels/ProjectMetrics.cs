namespace ProjectScannerSaveToSqlServer.DataModels
{
    public partial class ProjectMetrics
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        public int MaintainabilityIndex { get; set; }

        public int CyclomaticComplexity { get; set; }

        public int ClassCoupling { get; set; }

        public int DepthOfInheritance { get; set; }

        public int SourceLines { get; set; }

        public int ExecutableLines { get; set; }

        public byte UnitTestCodeCoverage { get; set; }

        public int? LastRunTotalWarnings { get; set; }

        public int? LastRunTotalErrors { get; set; }

        public virtual File File { get; set; }
    }
}
