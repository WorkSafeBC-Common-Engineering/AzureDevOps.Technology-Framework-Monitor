using ProjectData.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProjectData
{
    public class Organization
    {
        #region Private members

        private readonly List<Project> projects = new List<Project>();

        #endregion

        #region Constructors

        public Organization(ProjectSource source, string name, string url)
        {
            Source = source;
            Name = name;
            Url = url;
        }

        #endregion

        #region Public Properties

        public ProjectSource Source { get; private set; }

        public string Name { get; private set; }

        public string Url { get; private set; }

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
