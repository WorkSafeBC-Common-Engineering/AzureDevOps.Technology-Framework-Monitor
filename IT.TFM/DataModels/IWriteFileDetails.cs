namespace RepoScan.DataModels
{
    public interface IWriteFileDetails
    {
        void Write(FileDetails item, bool forceDetails);

        void Delete(FileDetails item);
    }
}
