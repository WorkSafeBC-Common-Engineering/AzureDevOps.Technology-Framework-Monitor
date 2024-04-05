using System.Data.Entity;

namespace ProjectScannerSaveToSqlServer.DataModels
{
    public partial class ProjectScannerDB : DbContext
    {
        public ProjectScannerDB(string connection)
            : base(connection) { }
    }
}
