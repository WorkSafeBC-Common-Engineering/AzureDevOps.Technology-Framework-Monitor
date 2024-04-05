using ProjectScannerSaveToSqlServer.DataModels;

using System;
using System.Diagnostics;
using System.Threading;

namespace ProjectScannerSaveToSqlServer
{
    public abstract class DbCore : IDisposable
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
        protected Guid instanceId;
        private static long instanceCount = 0;

        #endregion

        #region Protected Methods

        protected void Initialize(string configuration)
        {
            instanceId = Guid.NewGuid();
            Interlocked.Increment(ref instanceCount);
#if DEBUG
            Console.WriteLine($"\t >>> DbCore - Initialize: {instanceId}, count = {Interlocked.Read(ref instanceCount)}");
#endif

            connection = configuration;
            context = GetConnection();
        }

        protected ProjectScannerDB GetConnection()
        {
            return new ProjectScannerDB(connection);
        }

        protected void Close()
        {
            var st = new StackTrace();
            var st1 = new StackTrace(new StackFrame(true));
#if DEBUG
            Console.WriteLine($"Stack Trace for Main: {st1}");
            Console.WriteLine(st.ToString());
#endif

            context.Dispose();
            organizationId = 0;
            projectId = 0;
            repositoryId = 0;
            fileId = 0;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            ((IDisposable)context).Dispose();
            GC.SuppressFinalize(this);

            Interlocked.Decrement(ref instanceCount);
#if DEBUG
            Console.WriteLine($"\t >>> DbCore - Close: {instanceId}, count = {Interlocked.Read(ref instanceCount)}");
#endif
            instanceId = Guid.Empty;
        }

        #endregion
    }
}
