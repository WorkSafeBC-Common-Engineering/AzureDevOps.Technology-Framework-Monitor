using ProjectData;

namespace RepoScan.DataModels
{
    public static class StorageFactory
    {
        public static IReadFileDetails GetFileDetailsReader()
        {
            return ServiceProviderAccessor.GetRequiredService<IReadFileDetails>();
        }

        public static IReadFileItem GetFileItemReader()
        {
            return ServiceProviderAccessor.GetRequiredService<IReadFileItem>();
        }

        public static IReadRepoList GetRepoListReader()
        {
            return ServiceProviderAccessor.GetRequiredService<IReadRepoList>();
        }

        public static IWriteFileDetails GetFileDetailsWriter()
        {
            return ServiceProviderAccessor.GetRequiredService<IWriteFileDetails>();
        }

        public static IWriteFileItem GetFileItemWriter()
        {
            return ServiceProviderAccessor.GetRequiredService<IWriteFileItem>();
        }

        public static IWriteRepoList GetRepoListWriter()
        {
            return ServiceProviderAccessor.GetRequiredService<IWriteRepoList>();
        }

        public static IReadPipelines GetPipelineReader()
        {
            return ServiceProviderAccessor.GetRequiredService<IReadPipelines>();
        }

        public static IWritePipeline GetPipelineWriter()
        {
            return ServiceProviderAccessor.GetRequiredService<IWritePipeline>();
        }

        public static IReadNuGet GetNuGetReader()
        {
            return ServiceProviderAccessor.GetRequiredService<IReadNuGet>();
        }

        public static IWriteNuGet GetNuGetWriter()
        {
            return ServiceProviderAccessor.GetRequiredService<IWriteNuGet>();
        }
    }
}
