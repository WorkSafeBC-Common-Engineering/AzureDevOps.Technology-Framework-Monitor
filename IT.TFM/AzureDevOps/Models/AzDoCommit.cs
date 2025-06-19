namespace AzureDevOps.Models
{
    internal class AzDoCommit
    {
        public string CommitId { get; set; } = string.Empty;

        public Committer Committer { get; set; } = new Committer();
    }

    internal class Committer
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Date { get; set; }
    }

    internal class AzDoCommitList
    {
        public int Count { get; set; }

        public AzDoCommit[] Value { get; set; } = [];
    }
}
