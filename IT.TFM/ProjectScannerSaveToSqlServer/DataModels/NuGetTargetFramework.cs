namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class NuGetTargetFramework
    {
        public int Id { get; set; }

        public int NuGetPackageId { get; set; }

        public string Framework { get; set; }

        public string Version { get; set; }

        public virtual NuGetPackage NuGetPackage { get; set; }
    }
}
