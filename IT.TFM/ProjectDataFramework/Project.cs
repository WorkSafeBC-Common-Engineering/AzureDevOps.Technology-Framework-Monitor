using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectData
{
    public class Project
    {
        #region Private Members

        private readonly List<Repository> repositories = new List<Repository>();

        #endregion

        #region Constructors

        public Project() { }

        public Project (string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public Guid Id { get; set; }

        public string Abbreviation { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdate { get; set; }

        public long Revision { get; set; }

        public string State { get; set; }

        public string Url { get; set; }

        public string Visibility { get; set; }

        public bool Deleted { get; set; }

        public bool NoScan { get; set; }

        public IEnumerable<Repository> Repositories
        {
            get { return repositories.AsEnumerable(); }
        }

        #endregion

        #region Public Methods

        public void AddRepository(Repository repository)
        {
            repositories.Add(repository);
        }

        #endregion
    }
}
