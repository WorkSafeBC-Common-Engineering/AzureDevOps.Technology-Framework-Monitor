using System;

namespace ProjectData.Interfaces
{
    public interface IStorageWriter
    {
        void Initialize(string configuration);

        void SaveOrganization(Organization organization);

        void SaveProject(Project project);

        void SaveRepository(Repository repository);

        void SavePipeline(Pipeline pipeline);

        void SaveRelease(Release release);

        void UpdatePipelineFileId(int pipelineId, string repositoryId, string fileId);

        void SaveFile(FileItem file, Guid repoId, bool saveDetails, bool forceDetails);

        void DeleteFile(FileItem file, Guid repoId);

        void DeletePipeline(int pipelineId);

        void Close();
    }
}
