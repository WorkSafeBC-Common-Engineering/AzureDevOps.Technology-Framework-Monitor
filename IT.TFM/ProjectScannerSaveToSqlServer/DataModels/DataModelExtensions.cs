using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public partial class ProjectScannerDB : DbContext
    {
        public ProjectScannerDB(string connection)
            : base(connection) { }
    }
}
