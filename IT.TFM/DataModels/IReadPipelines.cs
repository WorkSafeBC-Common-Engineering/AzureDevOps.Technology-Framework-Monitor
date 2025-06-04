using System;
using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadPipelines
    {
        IEnumerable<YamlPipeline> ReadYamlPipelines();

        IEnumerable<int> GetPipelineIds(string projectId);

        IEnumerable<ProjectData.Pipeline> FindPipelines(Guid projectId, Guid repositoryId, string portfolio, string product);

        IEnumerable<ProjectData.Pipeline> FindPipelines(ProjectData.FileItem file);
    }
}
