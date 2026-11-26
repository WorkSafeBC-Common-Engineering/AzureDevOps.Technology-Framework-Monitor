using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectData
{
    public class DeploymentEnvironments(string name, bool active, string environmentName, string pool, string vmImage)
    {

        #region Constructors

        #endregion

        #region Public Properties

        public string Name { get; set; } = name;

        public bool Active { get; set; } = active;

        public string EnvironmentName { get; set; } = environmentName;

        public string Pool { get; set; } = pool;

        public string VmImage { get; set; } = vmImage;

        public Dictionary<string, string> Variables { get; private set; } = [];

        #endregion

        #region Public Methods

        

        #endregion
    }

    public class DeploymentSettings
    {
        public List<DeploymentEnvironments> Environments = [];

    }
}
