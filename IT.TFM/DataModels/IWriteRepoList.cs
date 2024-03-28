namespace RepoScan.DataModels
{
    public interface IWriteRepoList
    {
        int Write(RepositoryItem item, int projectId, bool repoOnly);
    }
}
