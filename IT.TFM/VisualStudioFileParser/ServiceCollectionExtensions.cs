using Microsoft.Extensions.DependencyInjection;

using Parser.Interfaces;

namespace VisualStudioFileParser
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualStudioFileParsers(this IServiceCollection services)
        {
            services.AddKeyedTransient<IFileParser>("VSSolution", (_, _) => new SolutionParser());
            services.AddKeyedTransient<IFileParser>("VSXSolution", (_, _) => new SolutionXParser());
            services.AddKeyedTransient<IFileParser>("CSProject", (_, _) => new CSProjectParser());
            services.AddKeyedTransient<IFileParser>("VBProject", (_, _) => new CSProjectParser());
            services.AddKeyedTransient<IFileParser>("VB6Project", (_, _) => new VB6ProjectParser());
            services.AddKeyedTransient<IFileParser>("SqlProject", (_, _) => new SqlProjectParser());
            services.AddKeyedTransient<IFileParser>("VSConfig", (_, _) => new ConfigParser());

            return services;
        }
    }
}
