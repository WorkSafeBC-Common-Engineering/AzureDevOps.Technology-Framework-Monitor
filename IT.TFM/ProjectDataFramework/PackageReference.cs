namespace ProjectData
{
    [System.Diagnostics.DebuggerDisplay("{ToDebugString()}")]
    public class PackageReference
    {
        public string PackageType { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }

        public string VersionComparator { get; set; }

        public string FrameworkVersion { get; set; }

        public string ToDebugString()
        {
            return $"Package: {Id}, Version: {Version}, Package Type = {PackageType}";
        }
    }
}
