using Parameters;

using ProjectData;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;

namespace YamlFileParser
{
    internal class PipelineDeploymentParser
    {
        #region Private Members

        // The following identify sections in the config.yml file
        private static readonly string[] sectionHeaders = 
            [
            "development",
            "systemTest",
            "staging",
            "production",
            "productionCanadaCentral",
            "productionCanadaEast"
            ];

        private const string valueLine = "value:";

        // variables common to each section/environment
        private const string activeLine = "StageActive";

        private const string envNameLine = "StageEnvName";
        
        private const string poolLine = "StagePool";
        
        private const string vmImageLine = "StageVmImage";

        // some specific settings
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

        private static Dictionary<string, Tuple<string, string>[]> GetConfigSections(string[] content)
        {
            var sections = new Dictionary<string, Tuple<string, string>[]>();
            var sectionEntries = new List<Tuple<string, string>>();

            foreach (var header in sectionHeaders)
            {
                for (var index = 0; index < content.Length; index++)
                {
                    var line = content[index];

                    if (line.StartsWith(valueLine))
                    {
                        continue;
                    }

                    if (line.StartsWith(header, StringComparison.Ordinal))
                    {
                        var valueLine = content[++index]?.Trim() ?? string.Empty;
                        if (valueLine.StartsWith(valueLine))
                        {
                            var value = valueLine[(valueLine.IndexOf(':') + 1)..].Trim();
                            sectionEntries.Add(new Tuple<string, string>(line[header.Length..], value));
                        }
                    }
                }

                sections.Add(header, [.. sectionEntries]);
                sectionEntries.Clear();
            }

            return sections;
        }

        private static void GetCommonSettings(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            foreach (var key in sections.Keys)
            {
                var section = sections[key];
                if (section == null)
                {
                    continue;
                }

                string environmentName = section.SingleOrDefault(s => s.Item1.StartsWith(envNameLine))?.Item2 ?? string.Empty;
                string pool = section.SingleOrDefault(s => s.Item1.StartsWith(poolLine))?.Item2 ?? string.Empty;
                string vmImage = section.SingleOrDefault(s => s.Item1.StartsWith(vmImageLine))?.Item2 ?? string.Empty;

                if (!bool.TryParse(section.SingleOrDefault(s => s.Item1.StartsWith(activeLine))?.Item2 ?? bool.FalseString, out var active))
                {
                    active = false; 
                }

                file.DeploymentSettings.Environments.Add(new DeploymentEnvironments(key, active, environmentName, pool, vmImage));
            }
        }

        private static void Angular(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        { 
            throw new NotImplementedException();
        }

        private static void ApimApi(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void AppConfig(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void AzureFunction(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void AzureFunctionNodeJs(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void BatchConsole(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void BlobStorage(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void Kubernetes(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void LegacyAic(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void LegacyWebApp(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void NuGetPackage(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void SchemaRegistry(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WebApiPython(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WebApp(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WebAppPython(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WinAppAppSync(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        private static void WinService(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
