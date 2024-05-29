using ProjectData;

namespace RepoScan.DataModels
{
    public interface IWritePipeline
    {
        void Write(Pipeline pipeline);

        void WriteRelease(Release release);

        void LinkToFile(int pipelineId, string repositoryId, string filePath);

        void AddProperties(ProjectData.FileItem file);

        void Delete(int id);
    }
}
