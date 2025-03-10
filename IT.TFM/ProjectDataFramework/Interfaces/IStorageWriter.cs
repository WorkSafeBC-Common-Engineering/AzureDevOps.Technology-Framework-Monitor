﻿using System;
using System.Collections.Generic;

namespace ProjectData.Interfaces
{
    public interface IStorageWriter : IDisposable
    {
        void Initialize(string configuration);

        int SaveOrganization(Organization organization);

        int SaveProject(Project project, int organizationId);

        int SaveRepository(Repository repository, int projectId);

        void SavePipeline(Pipeline pipeline);

        void SaveRelease(Release release);

        void LinkPipelineToFile(int pipelineId, string repositoryId, string path);

        void SaveFile(FileItem file, Guid repoId, bool saveDetails, bool forceDetails);

        void DeleteFile(FileItem file, Guid repoId);

        void DeletePipeline(int pipelineId);

        int SaveNuGetPackage(NuGetPackage package);

        void CleanupNuGetPackages(IEnumerable<int> packageIds);

        void SaveMetrics(FileItem file, Metrics metrics);
    }
}
