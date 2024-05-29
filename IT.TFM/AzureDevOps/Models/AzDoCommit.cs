namespace AzureDevOps.Models
{
    internal class AzDoCommit
    {
        public string CommitId { get; set; } = string.Empty;
    }

    internal class AzDoCommitList
    {
        public int Count { get; set; }

        public AzDoCommit[] Value { get; set; } = [];
    }


}
