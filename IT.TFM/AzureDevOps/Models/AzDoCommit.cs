using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Models
{
    internal class AzDoCommit
    {
        public string CommitId { get; set; } = string.Empty;
    }

    internal class AzDoCommitList
    {
        public int Count { get; set; }

        public AzDoCommit[] Value { get; set; } = Array.Empty<AzDoCommit>();
    }


}
