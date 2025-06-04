using ProjectData.Interfaces;

using RepoScan.DataModels;
using DataStorage = Storage;
using System.Collections.Generic;
using System.Linq;
using ProjectData;
using System;

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

                        case "blueprintType":
                            pipeline.BlueprintApplicationType = value;
                            break;

                        case "suppressCD":
                            pipeline.SuppressCD = value.ToLower() != "false";
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

        IEnumerable<ProjectData.Pipeline> IReadPipelines.FindPipelines(Guid projectId, Guid repositoryId, string portfolio, string product)
        {
            using var reader = GetReader();

            var project = projectId.ToString("D");
            var repository = repositoryId.ToString("D");

            var pipelines = reader.FindPipelines(project, repository, portfolio, product);
            return pipelines;
        }

        IEnumerable<ProjectData.Pipeline> IReadPipelines.FindPipelines(ProjectData.FileItem file)
        {
            using var reader = GetReader();
            var pipelines = reader.GetPipelines(file.RepositoryId.ToString("D"), file.Path);
            return pipelines.ToArray().AsEnumerable();
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
