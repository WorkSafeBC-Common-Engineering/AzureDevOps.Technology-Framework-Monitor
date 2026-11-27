using Microsoft.Extensions.Primitives;

using Parameters;

using ProjectData;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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

        private const string tagAzureResourceAnonymous = "AzureResource.Anonymous";

        private const string tagAzureResourceAad = "AzureResource.Aad";

        private const string tagAzureResourceCiam = "AzureResource.Ciam";

        private const string tagKeyVault = "KeyVault";

        private const string tagKeyVaultApp = "KeyVault.AppSecrets";

        private const string tagKeyVaultAppFilter = "KeyVault.AppSecretsFilter";

        private const string tagKeyVaultAppSpecific = "KeyVault.AppSpecificSecrets";

        private const string tagKeyVaultAppSpecificFilter = "KeyVault.AppSpecificFilter";

        private const string tagTargetServers = "TargetServers";

        private const string tagAdmin = "AdminUser";

        private const string tagAppConfig = "AppConfigFile";

        private const string tagCmdFolder = "CmdFolder";

        private const string tagCybExecFolder = "CybExecFolder";

        private const string tagServerPath = "ServerPath";

        private const string tagEventSource = "EventSource";

        private const string tagContainer = "Container";

        private const string tagPrefix = "Prefix";

        private const string tagCleanTarget = "CleanTarget";

        private const string tagShortEnvironmentName = "Environment.ShortName";

        private const string tagNuGetFeed = "NuGetFeed";

        private const string tagNameSpace = "NameSpace";

        private const string tagSchemaGroup = "SchemaGroup";

        private const string tagAppType = "AppType";

        private const string tagAppName = "AppName";

        private const string tagAppNameShort = "AppName.Short";

        private const string tagServiceNames = "ServiceNames";

        private const string tagRestartService = "RestartService";

        private const string tagRootFolder = "RootFolder";

        private const string tagServiceFolder = "ServiceFolder";

        private const string tagAppFolder = "AppFolder";

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

                var envNameEntry = section.SingleOrDefault(s => s.Item1.Equals(envNameLine));
                string environmentName = envNameEntry?.Item2 ?? string.Empty;

                var poolEntry = section.SingleOrDefault(s => s.Item1.Equals(poolLine));
                string pool = poolEntry?.Item2 ?? string.Empty;

                var vmImageEntry = section.SingleOrDefault(s => s.Item1.Equals(vmImageLine));
                string vmImage = vmImageEntry?.Item2 ?? string.Empty;

                var activeEntry = section.SingleOrDefault(s => s.Item1.StartsWith(activeLine));
                if (!bool.TryParse(activeEntry?.Item2 ?? bool.FalseString, out var active))
                {
                    active = false; 
                }

                file.DeploymentSettings.Environments.Add(new DeploymentEnvironments(key, active, environmentName, pool, vmImage));

                // clear these so that they do not get processed again.
                var emptyEntry = new Tuple<string, string>(string.Empty, string.Empty);
                envNameEntry = emptyEntry;
                poolEntry = emptyEntry;
                vmImageEntry = emptyEntry;
                activeEntry = emptyEntry;
            }
        }

        private static void Angular(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "AzureSubscription";
            const string anonymousAppName = "AppNameAnonymous";
            const string aadAppName = "AppNameAad";
            const string ciamAppName = "AppNameCiam";
            const string keyVault = "KeyVaultName";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, anonymousAppName, tagAzureResourceAnonymous);
                AddEntry(environment, section.Value, aadAppName, tagAzureResourceAad);
                AddEntry(environment, section.Value, ciamAppName, tagAzureResourceCiam);
                AddEntry(environment, section.Value, keyVault, tagKeyVault);
            }
        }

        private static void ApimApi(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "StageAzureSubscription";
            const string envName = "StageEnvironment";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, envName, tagShortEnvironmentName);
            }
        }

        private static void AppConfig(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "AzureSubscription";
            const string appConfigName = "AppConfigName";
            const string envLabel = "Label";
            const string keyPrefix = "KeyPrefix";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, appConfigName, tagAppConfig);
                AddEntry(environment, section.Value, envLabel, tagShortEnvironmentName);
                AddEntry(environment, section.Value, keyPrefix, tagPrefix);
            }
        }

        private static void AzureFunction(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "StageAzRmWebDeployAzureSubscription";
            const string resourceGroup = "StageAzResourceGroup";
            const string webAppName = "StageAzRmWebDeployWebAppName";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, resourceGroup, tagAzureResourceGroup);
                AddEntry(environment, section.Value, webAppName, tagAzureResourceName);
            }
        }

        private static void AzureFunctionNodeJs(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "AzureSubscription";
            const string appName = "AppName";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, appName, tagAzureResourceName);
            }
        }

        private static void BatchConsole(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string kvSubscription = "StageKeyVaultSubscription";
            const string kvPipeline = "StageKeyVaultName";
            const string kvAppSecrets = "StageKeyVaultAppSecrets";
            const string kvAppSecretsFilter = "StageKeyVaultAppSecretsFilter";
            const string kvAppSpecific = "StageKeyVaultAppSpecificSecrets";
            const string kvAppSpecificFilter = "StageKeyVaultAppSpecificSecretsFilter";
            const string targetServers = "StageTargetServers";
            const string targetAdmin = "StageTargetAdminUser";
            const string appConfigName = "StageAppConfigName";
            const string cmdFolder = "StageCmdFolder";
            const string cybExecFolder = "StageCybExecFolder";
            const string serverPath = "StageServerPath";
            const string eventSource = "StageEventSource";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, kvSubscription, tagSubscription);
                AddEntry(environment, section.Value, kvPipeline, tagKeyVault);
                AddEntry(environment, section.Value, kvAppSecrets, tagKeyVaultApp);
                AddEntry(environment, section.Value, kvAppSecretsFilter, tagKeyVaultAppFilter);
                AddEntry(environment, section.Value, kvAppSpecific, tagKeyVaultAppSpecific);
                AddEntry(environment, section.Value, kvAppSpecificFilter, tagKeyVaultAppSpecificFilter);
                AddEntry(environment, section.Value, targetServers, tagTargetServers);
                AddEntry(environment, section.Value, targetAdmin, tagAdmin);
                AddEntry(environment, section.Value, appConfigName, tagAppConfig);
                AddEntry(environment, section.Value, cmdFolder, tagCmdFolder);
                AddEntry(environment, section.Value, cybExecFolder, tagCybExecFolder);
                AddEntry(environment, section.Value, serverPath, tagServerPath);
                AddEntry(environment, section.Value, eventSource, tagEventSource);
            }
        }

        private static void BlobStorage(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "AzureSubscription";
            const string storageAccount = "StorageAccountName";
            const string container = "containerName";
            const string destinationPrefix = "DestinationPrefix";
            const string cleanDestination = "CleanDestination";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, storageAccount, tagAzureResourceName);
                AddEntry(environment, section.Value, container, tagContainer);
                AddEntry(environment, section.Value, destinationPrefix, tagPrefix);
                AddEntry(environment, section.Value, cleanDestination, tagCleanTarget);
            }
        }

        private static void Kubernetes(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            // Only the common deployment variables are used in this blueprint - no further parsing required.
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
            const string targetFeed = "TargetFeed";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, targetFeed, tagNuGetFeed);
            }
        }

        private static void SchemaRegistry(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "StageAzServiceConnection";
            const string resourceGroup = "StageAzResourceGroup";
            const string nameSpace = "StageAzNameSpace";
            const string schemaGroup = "StageAzSchemaGroup";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, resourceGroup, tagAzureResourceGroup);
                AddEntry(environment, section.Value, nameSpace, tagNameSpace);
                AddEntry(environment, section.Value, schemaGroup, tagSchemaGroup);
            }
        }

        private static void WebApiPython(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "StageAzureSubscription";
            const string appName = "StageAppName";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, appName, tagAzureResourceName);
            }
        }

        private static void WebApp(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "AzureSubscription";
            const string appType = "AppType";
            const string appName = "AppName";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, appType, tagAppType);
                AddEntry(environment, section.Value, appName, tagAzureResourceName);
            }
        }

        private static void WebAppPython(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string subscription = "StageAzureSubscription";
            const string appName = "StageAppName";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, appName, tagAzureResourceName);
            }
        }

        private static void WinAppAppSync(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string keyvault = "StageKeyVaultName";
            const string subscription = "StageKeyVaultSubscription";
            const string targetServers = "StageTargetServers";
            const string targetAdmin = "StageTargetAdminUser";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, keyvault, tagKeyVault);
                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, targetServers, tagTargetServers);
                AddEntry(environment, section.Value, targetAdmin, tagAdmin);
            }
        }

        private static void WinService(FileItem file, Dictionary<string, Tuple<string, string>[]> sections)
        {
            const string keyvault = "StageKeyVaultName";
            const string subscription = "StageKeyVaultSubscription";
            const string serverName = "ServerName";
            const string userName = "UserName";
            const string appRootFolder = "AppRootFolder";
            const string serviceFolder = "ServiceFolder";
            const string appFolder = "AppFolder";
            const string webAppShortName = "WebAppShortName";
            const string webAppName = "WebAppName";
            const string serviceNames = "ServiceNames";
            const string restartService = "RestartService";

            foreach (var section in sections)
            {
                var environment = file.DeploymentSettings.Environments.SingleOrDefault(env => env.Name == section.Key);
                if (environment == null)
                {
                    continue;
                }

                AddEntry(environment, section.Value, keyvault, tagKeyVault);
                AddEntry(environment, section.Value, subscription, tagSubscription);
                AddEntry(environment, section.Value, serverName, tagTargetServers);
                AddEntry(environment, section.Value, userName, tagAdmin);
                AddEntry(environment, section.Value, appRootFolder, tagRootFolder);
                AddEntry(environment, section.Value, serviceFolder, tagServiceFolder);
                AddEntry(environment, section.Value, appFolder, tagAppFolder);
                AddEntry(environment, section.Value, webAppShortName, tagAppNameShort);
                AddEntry(environment, section.Value, webAppName, tagAppName);
                AddEntry(environment, section.Value, serviceNames, tagServiceNames);
                AddEntry(environment, section.Value, restartService, tagRestartService);
            }
        }

        private static void AddEntry(DeploymentEnvironments environment, Tuple<string, string>[] section, string variableName, string variableKey)
        {
            var entry = section.SingleOrDefault(e => e.Item2.Equals(variableName));
            if (entry != null)
            {
                environment.Variables.Add(variableKey, entry.Item2);
            }
        }

        #endregion
    }
}
