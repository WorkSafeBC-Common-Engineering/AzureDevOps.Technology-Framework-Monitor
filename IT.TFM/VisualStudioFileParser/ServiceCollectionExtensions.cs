using Microsoft.Extensions.DependencyInjection;

using Parser.Interfaces;

namespace VisualStudioFileParser
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualStudioFileParsers(this IServiceCollection services)
        {
            services.AddKeyedTransient<IFileParser>("VSSolution", (_, _) => new SolutionParser());
            services.AddKeyedTransient<IFileParser, SolutionXParser>("VSXSolution");
            services.AddKeyedTransient<IFileParser, CSProjectParser>("CSProject");
            services.AddKeyedTransient<IFileParser, CSProjectParser>("VBProject");
            services.AddKeyedTransient<IFileParser>("VB6Project", (_, _) => new VB6ProjectParser());
            services.AddKeyedTransient<IFileParser, SqlProjectParser>("SqlProject");
            services.AddKeyedSingleton<IFileParser, ConfigParser>("VSConfig");

            return services;
        }
    }
}
