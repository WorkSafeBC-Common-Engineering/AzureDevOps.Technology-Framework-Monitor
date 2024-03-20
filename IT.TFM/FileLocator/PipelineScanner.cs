using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoScan.FileLocator
{
    public static class PipelineScanner
    {
        #region Private Members

        private static IEnumerable<YamlPipeline> yamlPipelines = null;
        private static readonly object pipelineReaderLock = new();

        private static readonly IWritePipeline writer = StorageFactory.GetPipelineWriter();

        #endregion

        #region Public Methods

        public static async Task ScanAsync(IScanner scanner, Guid projectId, string repositoryId)
        {
            var pipelines = await scanner.Pipelines(projectId, repositoryId);
            var pipelineWriter = StorageFactory.GetPipelineWriter();
            foreach (var pipeline in pipelines)
            {
                // Odd bit here: the file paths always have a leading slash, but not the paths in the YAML pipelines. So if not present, I will add the slash in those cases.
                // Otherwise we won't be able to link the yaml pipeline to a file
                if (pipeline.Type == "yaml" && !pipeline.Path.StartsWith('/'))
                {
                    pipeline.Path = $"/{pipeline.Path}";
                }

                pipelineWriter.Write(pipeline);
            }
        }

        public static void LinkPipelineToFile(int totalThreads, string repositoryId)
        {
            Settings.Initialize();

            var repoReader = StorageFactory.GetRepoListReader();
            var fileReader = StorageFactory.GetFileItemReader();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = totalThreads
            };

            var repoIds = new List<string>();
            if (string.IsNullOrEmpty(repositoryId))
            {
                repoIds.AddRange(repoReader.GetRepositoryIds());
            }
            else
            {
                repoIds.Add(repositoryId);
            }

            foreach (var repoId in repoIds)
            {
                var yamlFiles = fileReader.YamlRead(repoId).ToArray();

                Parallel.ForEach(yamlFiles, options, (file) =>
                {
                    LinkYamlPipeline(repoId, file.Path, file.Id);
                });
            }
        }

        #endregion

        #region Private Methods

        public static void LinkYamlPipeline(string repositoryId, string filePath, string fileId)
        {
            lock (pipelineReaderLock)
            {
                if (yamlPipelines == null)
                {
                    IReadPipelines reader = StorageFactory.GetPipelineReader();
                    yamlPipelines = reader.ReadYamlPipelines();
                }
            }

            Debug.WriteLine($"Link Yaml Pipeline: Repository ID: {repositoryId}, Path: {filePath}, FileID: {fileId}");

            // It is possible that multiple pipelines can be created from the same YAML file, so need to process each case
            var pipelines = yamlPipelines.Where(p => p.RepositoryId != null
                                                  && p.RepositoryId.Equals(repositoryId, StringComparison.InvariantCultureIgnoreCase)
                                                  && p.Path.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));

            foreach (var pipeline in pipelines)
            {
                Debug.WriteLine($"Pipeline to File match: {pipeline}");
                if (!fileId.Equals(pipeline.FileId) )
                {
                    writer.UpdateFileId(pipeline.PipelineId, pipeline.RepositoryId, fileId);
                }
            }
        }

        #endregion
    }
}
