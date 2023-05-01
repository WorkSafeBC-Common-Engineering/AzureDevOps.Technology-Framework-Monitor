using ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.DataModels
{
    public class RepositoryItem
    {
        public Guid ScanID { get; set; }

        public DateTime ScanTime { get; set; }

        public ProjectSource Source { get; set; }

        public string OrgName { get; set; }

        public string OrgUrl { get; set; }

        public string ProjectName { get; set; }

        public Guid ProjectId { get; set; }

        public string ProjectAbbreviation { get; set; }

        public string ProjectDescription { get; set; }

        public DateTime ProjectLastUpdate { get; set; }

        public long ProjectRevision { get; set; }

        public string ProjectState { get; set; }

        public string ProjectUrl { get; set; }

        public string ProjectVisibility { get; set; }

        public bool ProjectIsDeleted { get; set; }

        public Guid RepositoryId { get; set; }

        public string RepositoryName { get; set; }

        public string RepositoryDefaultBranch { get; set; }

        public bool RepositoryIsFork { get; set; }

        public long RepositorySize { get; set; }

        public string RepositoryUrl { get; set; }

        public string RepositoryRemoteUrl { get; set; }

        public string RepositoryWebUrl { get; set; }

        public bool IsDeleted { get; set; }
    }
}
