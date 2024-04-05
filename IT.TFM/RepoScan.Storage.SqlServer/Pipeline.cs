﻿using ProjectData.Interfaces;

using RepoScan.DataModels;
using DataStorage = Storage;
using System.Collections.Generic;
using System.Linq;
using ProjectData;

namespace RepoScan.Storage.SqlServer
{
    internal class Pipeline : IReadPipelines, IWritePipeline
    {
        #region IWritePipeline Members

        void IWritePipeline.Write(ProjectData.Pipeline pipeline)
        {
            using var writer = GetWriter();

            writer.SavePipeline(pipeline);
        }

        void IWritePipeline.WriteRelease(Release release)
        {
            using var writer = GetWriter();

            writer.SaveRelease(release);
        }

        void IWritePipeline.LinkToFile(int pipelineId, string repositoryId, string filePath)
        {
            using var writer = GetWriter();
            writer.LinkPipelineToFile(pipelineId, repositoryId, filePath);
        }

        void IWritePipeline.AddProperties(ProjectData.FileItem file)
        {
            using var reader = GetReader();
            using var writer = GetWriter();

            var pipelines = reader.GetPipelines(file.RepositoryId.ToString("D"), file.Path);
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
            using var writer = GetWriter();
            writer.DeletePipeline(id);
        }

        #endregion

        #region IReadPipelines Implementation

        IEnumerable<YamlPipeline> IReadPipelines.ReadYamlPipelines()
        {
            using var reader = GetReader();
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
            using var reader = GetReader();
            return reader.GetPipelineIdsForProject(projectId).ToArray();
        }

        #endregion

        #region Private Methods

        private static IStorageWriter GetWriter()
        {
            return DataStorage.StorageFactory.GetStorageWriter();
        }

        private static IStorageReader GetReader()
        {
            return DataStorage.StorageFactory.GetStorageReader();
        }

        #endregion
    }
}
