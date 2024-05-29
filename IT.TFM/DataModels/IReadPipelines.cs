using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadPipelines
    {
        IEnumerable<YamlPipeline> ReadYamlPipelines();

        IEnumerable<int> GetPipelineIds(string projectId);
    }
}
