using ProjectData.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProjectData
{
    public class Organization(ProjectSource source, string name, string url)
    {
        #region Private members

        private readonly List<Project> projects = [];

        #endregion
        #region Constructors

        #endregion

        #region Public Properties

        public ProjectSource Source { get; private set; } = source;

        public string Name { get; private set; } = name;

        public string Url { get; private set; } = url;

        public IEnumerable<Project> Projects
        {
            get { return projects.AsEnumerable();  }
        }

        #endregion

        #region Public Methods

        public Project AddProject(string name, Guid id)
        {
            var project = new Project(name, id);
            projects.Add(project);

            return project;
        }

        public void AddProject(Project project)
        {
            projects.Add(project);
        }

        #endregion
    }
}
