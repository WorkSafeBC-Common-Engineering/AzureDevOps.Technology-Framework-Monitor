using Microsoft.Extensions.DependencyInjection;

using RepoScan.DataModels;

namespace RepoScan.Storage.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepoScanSqlServerStorage(this IServiceCollection services)
        {
            services.AddTransient<IReadFileItem, FileItem>();
            services.AddTransient<IReadRepoList, RepoList>();
            services.AddTransient<IWriteFileDetails, FileDetails>();
            services.AddTransient<IWriteFileItem, FileItem>();
            services.AddTransient<IWriteRepoList, RepoList>();
            services.AddTransient<IReadPipelines>(_ => new Pipeline());
            services.AddTransient<IWritePipeline>(_ => new Pipeline());
            services.AddTransient<IReadNuGet>(_ => new NuGet());
            services.AddTransient<IWriteNuGet>(_ => new NuGet());

            return services;
        }
    }
}
