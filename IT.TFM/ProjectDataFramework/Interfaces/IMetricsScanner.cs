namespace ProjectData.Interfaces
{
    public interface IMetricsScanner
    {
        Metrics? Get(FileItem file, string basePath);
    }
}
