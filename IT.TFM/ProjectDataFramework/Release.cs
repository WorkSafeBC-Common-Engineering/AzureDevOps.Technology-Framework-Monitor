using System;

namespace ProjectData
{
    public class Release : Pipeline
    {
        public string Source { get; set; }

        public string CreatedByName { get; set; }

        public string CreatedById { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string ModifiedByName { get; set; }

        public string ModifiedById { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDisabled { get; set; }

        public int LastReleaseId { get; set; }

        public string LastReleaseName { get; set; }

        public Artifact[] Artifacts { get; set; }
    }

    public class Artifact
    {
        public string SourceId { get; set; }

        public string Type { get; set; }

        public string Alias { get; set; }

        public string Url { get; set; }

        public string DefaultVersionType { get; set; }

        public string DefinitionId { get; set; }

        public string DefinitionName { get; set; }

        public string Project { get; set; }

        public string ProjectId { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsRetained { get; set; }
    }
}
