using ProjectData;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class Pipeline
    {
        public int Id { get; set; }

        public int PipelineId { get; set; }

        public int RepositoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Folder { get; set; } = string.Empty;

        public int Revision { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PipelineType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string QueueStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Quality { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public virtual Repository Repository { get; set; }
    }
}
