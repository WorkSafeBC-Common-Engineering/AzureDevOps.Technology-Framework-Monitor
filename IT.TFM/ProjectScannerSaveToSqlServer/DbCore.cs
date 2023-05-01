using ProjectScannerSaveToSqlServer.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer
{
    public abstract class DbCore
    {
        #region Protected Members

        // File Reference Type IDs
        protected const int RefTypeFile = 1;
        protected const int RefTypeUrl = 2;
        protected const int RefTypePkg = 3;

        // File Property Types
        protected const int PropertyTypeProperty = 1;
        protected const int PropertyTypeFilteredItem = 2;

        // Keep track of the current data while processing
        protected int organizationId = 0;

        protected int projectId = 0;

        protected int repositoryId = 0;

        protected int fileId = 0;

        protected string connection;
        protected ProjectScannerDB context;

        #endregion

        #region Protected Methods

        protected void Initialize(string configuration)
        {
            connection = $"name={configuration}";
            context = GetConnection();
        }

        protected ProjectScannerDB GetConnection()
        {
            return new ProjectScannerDB(connection);
        }

        protected void Close()
        {
            context.Dispose();
            organizationId = 0;
            projectId = 0;
            repositoryId = 0;
            fileId = 0;
        }

        #endregion
    }
}
