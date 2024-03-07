using ProjectData.Interfaces;

using RepoScan.DataModels;
using DataStorage = Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RepoScan.Storage.SqlServer
{
    internal class Pipeline : IWritePipeline
    {
        #region Private Members

        private IStorageWriter sqlWriter = null;

        #endregion

        #region IWritePipeline Members

        void IWritePipeline.Write(ProjectData.Pipeline pipeline)
        {
            var writer = GetWriter();

            writer.SavePipeline(pipeline);
        }

        #endregion

        #region Private Methods

        private IStorageWriter GetWriter()
        {
            sqlWriter ??= DataStorage.StorageFactory.GetStorageWriter();

            return sqlWriter;
        }

        #endregion
    }
}
