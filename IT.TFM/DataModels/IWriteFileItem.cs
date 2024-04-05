namespace RepoScan.DataModels
{
    public interface IWriteFileItem
    {
        void Write(FileItem item, bool saveDetails, bool forceDetails);

        void Delete(FileItem item);
    }
}
