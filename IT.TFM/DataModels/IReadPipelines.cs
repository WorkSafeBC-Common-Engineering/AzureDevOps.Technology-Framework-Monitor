using System;
using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadPipelines
    {
        IEnumerable<YamlPipeline> ReadYamlPipelines();

        IEnumerable<int> GetPipelineIds(string projectId);

        ProjectData.Pipeline? FindPipeline(Guid projectId, Guid repositoryId, string portfolio, string product);
    }
}
