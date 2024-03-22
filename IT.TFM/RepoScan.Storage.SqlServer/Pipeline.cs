using ProjectData.Interfaces;

using RepoScan.DataModels;
using DataStorage = Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ConfidentialSettings;
using ProjectData;

namespace RepoScan.Storage.SqlServer
{
    internal class Pipeline : IReadPipelines, IWritePipeline
    {
        #region Private Members

        private IStorageWriter sqlWriter = null;
        private IStorageReader sqlReader = null;

        #endregion

        #region IWritePipeline Members

        void IWritePipeline.Write(ProjectData.Pipeline pipeline)
        {
            var writer = GetWriter();

            writer.SavePipeline(pipeline);
        }

        void IWritePipeline.WriteRelease(Release release)
        {
            var writer = GetWriter();

            writer.SaveRelease(release);
        }

        void IWritePipeline.UpdateFileId(int pipelineId, string repositoryId, string fileId)
        {
            var writer = GetWriter();
            writer.UpdatePipelineFileId(pipelineId, repositoryId, fileId);
        }

        void IWritePipeline.AddProperties(ProjectData.FileItem file)
        {
            var reader = GetReader();
            var writer = GetWriter();

            var pipelines = reader.GetPipelines(file.RepositoryId.ToString("D"), file.Id, file.Path);
            foreach (var pipeline in pipelines)
            {
                foreach (var key in file.PipelineProperties.Keys)
                {
                    var value = file.PipelineProperties[key];
                    switch (key)
                    {
                        case "template":
                            pipeline.YamlType = value;
                            break;

                        case "portfolio":
                            pipeline.Portfolio = value;
                            break;

                        case "product":
                            pipeline.Product = value;
                            break;
                    }
                }

                writer.SavePipeline(pipeline);
            }

        }

        void IWritePipeline.Delete(int id)
        {
            var writer = GetWriter();
            writer.DeletePipeline(id);
        }

        #endregion

        #region IReadPipelines Implementation

        IEnumerable<YamlPipeline> IReadPipelines.ReadYamlPipelines()
        {
            var reader = GetReader();
            var pipelines = reader.GetPipelines(ProjectData.Pipeline.pipelineTypeYaml);
            return pipelines.Select(p => new YamlPipeline
                                   {
                                       PipelineId = p.Id,
                                       RepositoryId = p.RepositoryId,
                                       Name = p.Name,
                                       Path = p.Path,
                                       FileId = p.FileId
                                   })
                            .ToArray()
                            .AsEnumerable();
        }

        IEnumerable<int> IReadPipelines.GetPipelineIds(string projectId)
        {
            var reader = GetReader();
            return reader.GetPipelineIdsForProject(projectId);
        }

        #endregion

        #region Private Methods

        private IStorageWriter GetWriter()
        {
            sqlWriter ??= DataStorage.StorageFactory.GetStorageWriter();
            return sqlWriter;
        }

        private IStorageReader GetReader()
        {
            sqlReader ??= DataStorage.StorageFactory.GetStorageReader();
            return sqlReader;
        }

        #endregion
    }
}
