using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace YamlFileParser
{
    internal class PipelineDeploymentParser
    {
        #region Private Members

        private const string tagSubscription = "Subscription";

        private const string tagAzureResourceGroup = "ResourceGroup";

        private const string tagAzureResourceName = "AzureResource";

        #endregion

        #region Internal Methods

        internal static void Parse(FileItem file, string[] content)
        {
            var sections = GetConfigSections(content);

            GetCommonSettings(file, sections);

            var blueprint = Enum.Parse<BlueprintType>(file.PipelineProperties["blueprintType"]);

            switch (blueprint)
            {
                case BlueprintType.Angular:
                    Angular(file, sections);
                    break;

                case BlueprintType.ApimApi:
                    ApimApi(file, sections);
                    break;

                case BlueprintType.AppConfig:
                    AppConfig(file, sections);
                    break;

                case BlueprintType.AzureFunction:
                    AzureFunction(file, sections);
                    break;

                case BlueprintType.AzureFunctionNodeJs:
                    AzureFunctionNodeJs(file, sections);
                    break;

                case BlueprintType.BatchConsole:
                    BatchConsole(file, sections);
                    break;

                case BlueprintType.BlobStorage:
                    BlobStorage(file, sections);
                    break;

                case BlueprintType.Kubernetes:
                    Kubernetes(file, sections);
                    break;

                case BlueprintType.LegacyAic:
                    LegacyAic(file, sections);
                    break;

                case BlueprintType.LegacyWebApp:
                    LegacyWebApp(file, sections);
                    break;

                case BlueprintType.NugetPackage:
                    NuGetPackage(file, sections);
                    break;

                case BlueprintType.SchemaRegistry:
                    SchemaRegistry(file, sections);
                    break;

                case BlueprintType.WebApiPython:
                    WebApiPython(file, sections);
                    break;

                case BlueprintType.WebApp:
                    WebApp(file, sections);
                    break;

                case BlueprintType.WebAppPython:
                    WebAppPython(file, sections);
                    break;

                case BlueprintType.WinAppAppSync:
                    WinAppAppSync(file, sections);
                    break;

                case BlueprintType.WinService:
                    WinService(file, sections);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private static void GetCommonSettings(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, string[]> GetConfigSections(string[] content)
        {
            return [];
        }

        private static void Angular(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void ApimApi(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void AppConfig(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void AzureFunction(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void AzureFunctionNodeJs(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void BatchConsole(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void BlobStorage(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void Kubernetes(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void LegacyAic(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void LegacyWebApp(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void NuGetPackage(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void SchemaRegistry(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WebApiPython(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WebApp(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WebAppPython(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WinAppAppSync(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WinService(FileItem file, Dictionary<string, string[]> sections)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
