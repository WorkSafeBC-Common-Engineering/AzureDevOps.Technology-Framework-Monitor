using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevOps.Models
{
    public class AzDoFile
    {
        public string ObjectId { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;    

        public string CommitId { get; set; } = string.Empty;

        public bool IsFolder { get; set; } = false;
    }

    public class AzDoFileList
    {
        public int Count { get; set; }

        public AzDoFile[] Value { get; set; } = [];
    }
}
