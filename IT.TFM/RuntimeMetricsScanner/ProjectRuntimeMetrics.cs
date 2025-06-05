namespace RuntimeMetricsScanner
{
    internal class ProjectRuntimeMetrics
    {
        #region Public Properties

        public string ProjectPath { get; set; } = string.Empty;

        public int? TotalWarnings { get; set; } = null;

        public int ? TotalErrors { get; set; } = null;

        #endregion
    }
}