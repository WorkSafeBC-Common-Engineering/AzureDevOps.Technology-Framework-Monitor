﻿using ProjectData;
using ProjectData.Interfaces;

using ProjectScanner;

using RepoScan.DataModels;

using System;
using System.Collections.Generic;
using System.Linq;
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
            var pipelineReader = StorageFactory.GetPipelineReader();
            var pipelineIds = new List<int>(pipelineReader.GetPipelineIds(projectId.ToString()));

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

                pipelineIds.Remove(pipeline.Id);
            }

            // remove any pipelines that are not being returned from the Scanner, must have been deleted.
            foreach (var id in pipelineIds)
            {
                pipelineWriter.Delete(id);
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
                    LinkYamlPipeline(repoId, file.Path);
                });
            }
        }

        public static async Task ScanReleasesAsync(IScanner scanner, Guid projectId, string repositoryId)
        {
            var releases = await scanner.Releases(projectId, repositoryId);
            var pipelineWriter = StorageFactory.GetPipelineWriter();

            foreach (var pipeline in releases)
            {
                var release = pipeline as Release;
                pipelineWriter.WriteRelease(release);
            }
        }

        #endregion

        #region Private Methods

        private static void LinkYamlPipeline(string repositoryId, string filePath)
        {
            lock (pipelineReaderLock)
            {
                if (yamlPipelines == null)
                {
                    IReadPipelines reader = StorageFactory.GetPipelineReader();
                    yamlPipelines = reader.ReadYamlPipelines();
                }
            }

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"Link Yaml Pipeline: Repository ID: {repositoryId}, Path: {filePath}");
            }

            // It is possible that multiple pipelines can be created from the same YAML file, so need to process each case
            var pipelines = yamlPipelines.Where(p => p.RepositoryId != null
                                                  && p.RepositoryId.Equals(repositoryId, StringComparison.InvariantCultureIgnoreCase)
                                                  && p.Path.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));

            foreach (var pipeline in pipelines)
            {
                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"Pipeline to File match: {pipeline}");
                }

                writer.LinkToFile(pipeline.PipelineId, pipeline.RepositoryId, filePath);
            }
        }

        #endregion
    }
}
