using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public class Pipeline
    {
        public Pipeline()
        {
            ReleaseArtifacts = [];
        }

        public int Id { get; set; }

        public int PipelineId { get; set; }

        public int? ProjectId { get; set; }
        
        public int? RepositoryId { get; set; }

        public int? FileId { get; set; }

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

        [StringLength(20)]
        public string PipelineType { get; set; } = string.Empty;

        public string Path { get; set; } = null;

        public string YamlType { get; set; } = null;

        public string Portfolio { get; set; } = null;

        public string Product { get; set; } = null;

        public int BlueprintApplicationTypeId { get; set; } = 0;

        public string Source { get; set; }

        public string CreatedByName { get; set; }

        public string CreatedById { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public string ModifiedByName { get; set; }

        public string ModifiedById { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsDisabled { get; set; } = false;

        public bool SuppressCD { get; set; } = true;

        public int? LastReleaseId { get; set; }

        public string LastReleaseName { get; set; }

        public string Environments { get; set; }

        public string State { get; set; } = string.Empty;

        public string Result { get; set; } = string.Empty;

        public DateTime? LastRunStart { get; set; }

        public DateTime? LastRunEnd { get; set; }

        public string LastRunUrl { get; set; } = string.Empty;

        public virtual Repository Repository { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<ReleaseArtifact> ReleaseArtifacts { get; set; }

        public virtual PipelineType BlueprintType { get; set; }
    }
}
