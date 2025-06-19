using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Models
{
    public class AzDoPipelineRunLogs
    {
        public Log[] Logs { get; set; } = [];
        public string Url { get; set; } = string.Empty;
    }

    public class Log
    {
        public DateTime CreatedOn { get; set; }
        public int Id { get; set; }
        public DateTime LastChangedOn { get; set; }
        public int LineCount { get; set; }
        public string Url { get; set; } = string.Empty;

        public SignedUrl? SignedContent { get; set; }
    }

    public class  SignedUrl
    {
        public string Url { get; set; } = string.Empty;

        public DateTime SignatureExpires { get; set; }
    }
}
