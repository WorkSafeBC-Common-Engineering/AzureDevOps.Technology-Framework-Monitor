using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectData
{
    public class Repository
    {
        #region Private Members

        private readonly List<FileItem> files = [];

        #endregion

        #region Public Properties

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DefaultBranch { get; set; }

        public bool IsFork { get; set; }

        public long Size { get; set; }

        public string Url { get; set; }

        public string RemoteUrl { get; set; }

        public string WebUrl { get; set; }

        public IEnumerable<FileItem> Files
        {
            get { return files.AsEnumerable(); }
        }

        public long FileCount { get; set; }

        public int PipelineCount { get; set; }

        public string OrgName { get; set; }

        public bool Deleted { get; set; }

        public string LastCommitId { get; set; }

        public DateOnly? CreatedOn { get; set; }

        public DateOnly? LastUpdatedOn { get; set; }

        public bool NoScan { get; set; }

        #endregion

        #region Public Methods

        public void AddFiles(IEnumerable<FileItem> items)
        {
            files.AddRange(items);
        }

        #endregion
    }
}
