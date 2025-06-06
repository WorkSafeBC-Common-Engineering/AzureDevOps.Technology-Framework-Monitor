using System;

namespace ProjectData
{
    public class ProjectRuntimeMetrics : IEquatable<ProjectRuntimeMetrics>
    {
        #region Public Properties

        public string ProjectPath { get; set; } = string.Empty;

        public int TotalWarnings { get; set; }

        public int TotalErrors { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"Project: {ProjectPath} - {TotalWarnings} warnings, {TotalErrors} errors ";
        }

        #endregion

        #region IEquatable Implementation

        bool IEquatable<ProjectRuntimeMetrics>.Equals(ProjectRuntimeMetrics other)
        {
            if (other is null) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            
            return ProjectPath.Equals(other.ProjectPath, StringComparison.InvariantCultureIgnoreCase)
                && TotalWarnings == other.TotalWarnings
                && TotalErrors == other.TotalErrors;
        }

        public override int GetHashCode()
        {
            int hashProjectPath = ProjectPath == null ? 0 : ProjectPath.GetHashCode();
            int hashTotalWarnings = TotalWarnings.GetHashCode();
            int hashTotalErrors = TotalErrors.GetHashCode();

            return hashProjectPath ^ hashTotalWarnings ^ hashTotalErrors;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectRuntimeMetrics);
        }

        #endregion
    }
}