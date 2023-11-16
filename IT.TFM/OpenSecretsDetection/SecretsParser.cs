using Parser.Interfaces;
using ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenSecretsDetection
{
    public class SecretsParser : IContentParser
    {
        #region Private Members

        #region Used for Database Open Secrets

        private const string dbSecretProperty = "DB Open Secret";
        private const string connectionsPath = @"/configuration/connectionStrings/add";


        private const string connectionStringAttribute = "connectionString";
        private const string connectionName = "name";
        private readonly string[] connectionSecrets =
                                    [
                                        "password"
                                    ];

        #endregion

        #region Used for ApiKey Open Secrets

        private const string apiKeySecretProperty = "ApiKey Open Secret";
        private const string apiKeyPath = @"/configuration/system.serviceModel/behaviors/endpointBehaviors/behavior/httpApiKey";
        private const string es2KeyPath = @"/configuration/system.serviceModel/behaviors/endpointBehaviors/behavior/httpES2ApiKey";
        private const string useKeyVaultAttribute = "useKeyVault";
        private const string apiKeyAttribute = "apikey";
        private const string parentNode = "behavior";
        private const string parentNameAttribute = "name";

        #endregion

        #region Used for possible OAuth Secrets

        private const string oauthSecretProperty = "* OAuth Open Secret";
        private const string oauthSearchOn = "oauth";

        #endregion

        #region Search App Settings

        private const string appSettingsPath = @"/configuration/appSettings";
        private const string appSettingNode = "add";
        private const string appSettingNameAttribute = "key";
        private const string appSettingValueAttribute = "value";

        #endregion

        private const string KeyVaultTokenPrefix = "__";
        private const string KeyVaultTokenPostfix = "__";

        #endregion

        #region IContentParser Implementation

        void IContentParser.Parse(FileItem file, string[] content)
        {
            ScanForDbSecrets(file, content);
            ScanForApiKeySecrets(file, content);
            ScanForOAuthSecrets(file, content);
        }

        #endregion

        #region Private Methods

        //private void ScanForDbSecrets2(FileItem file, string[] content)
        //{
        //    if (file.FileType != FileItemType.VSConfig)
        //    {
        //        return;
        //    }

        //    bool inConnectionStrings = false;
        //    var connectionNames = new List<string>();

        //    foreach (var line in content)
        //    {
        //        if (line.Contains(startConnectionStrings))
        //        {
        //            inConnectionStrings = true;
        //            connectionNames.Clear();
        //            continue;
        //        }

        //        if (line.Contains(endConnectionStrings))
        //        {
        //            inConnectionStrings = false;

        //            if (connectionNames.Any())
        //            {
        //                var allNames = string.Join(", ", connectionNames.ToArray());
        //                file.AddProperty(dbSecretProperty, allNames);
        //            }

        //            continue;
        //        }

        //        if (inConnectionStrings && line.Contains(connectionStringAttribute))
        //        {
        //            var connection = GetAttribute(line, connectionStringAttribute).ToLower();

        //            foreach (var secret in connectionSecrets)
        //            {
        //                if (connection.Contains(secret))
        //                {
        //                    connectionNames.Add(GetAttribute(line, connectionName));
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

        private void ScanForDbSecrets(FileItem file, string[] content)
        {
            if (file.FileType != FileItemType.VSConfig)
            {
                return;
            }

            var connectionNames = new List<string>();

            var xmlDoc = GetXml(content);
            var nodes = xmlDoc.SelectNodes(connectionsPath);
            foreach (XmlNode node in nodes)
            {
                var nameAttribute = node[connectionName];
                var valueAttribute = node[connectionStringAttribute];

                var nameValue = nameAttribute == null ? string.Empty : nameAttribute.Value.Trim();
                var value = valueAttribute == null ? string.Empty : valueAttribute.Value.Trim();

                foreach (var secret in connectionSecrets)
                {
                    if (value.Contains(secret, StringComparison.CurrentCultureIgnoreCase))
                    {
                        connectionNames.Add(nameValue);
                        break;
                    }
                }

                if (connectionNames.Count != 0)
                {
                    var allConnections = string.Join(", ", [.. connectionNames]);
                    file.AddProperty(dbSecretProperty, allConnections);
                }
            }
        }

        private static void ScanForApiKeySecrets(FileItem file, string[] content)
        {
            if (file.FileType != FileItemType.VSConfig)
            {
                return;
            }

            var secretsList = new List<string>();
            var xmlDoc = GetXml(content);

            GetApiKeySecrets(xmlDoc, apiKeyPath, secretsList);
            GetApiKeySecrets(xmlDoc, es2KeyPath, secretsList);

            if (secretsList.Count != 0)
            {
                var allSecrets = string.Join(", ", [.. secretsList]);
                file.AddProperty(apiKeySecretProperty, allSecrets);
            }
        }

        private static void ScanForOAuthSecrets(FileItem file, string[] content)
        {
            if (file.FileType != FileItemType.VSConfig)
            {
                return;
            }

            var oAuthList = new List<string>();
            var xmlDoc = GetXml(content);
            SearchAppSettings(xmlDoc, oauthSearchOn, oAuthList);

            if (oAuthList.Count != 0)
            {
                var allOAuth = string.Join(", ", [.. oAuthList]);
                file.AddProperty(oauthSecretProperty, allOAuth);
            }
        }

        private static XmlDocument GetXml(string[] content)
        {
            var data = string.Concat(content);
            var xml = new XmlDocument();
            xml.LoadXml(data);

            return xml;
        }

        private static void GetApiKeySecrets(XmlDocument xmlDoc, string nodePath, List<string> secretsList)
        {
            var nodes = xmlDoc.SelectNodes(nodePath);
            if (nodes.Count == 0)
            {
                return;
            }

            foreach (XmlNode node in nodes)
            {
                var nameNode = node.ParentNode;
                string apiName = string.Empty;
                if (nameNode.Name.Equals(parentNode, StringComparison.InvariantCultureIgnoreCase))
                {
                    var apiNameAttribute = nameNode.Attributes[parentNameAttribute];
                    apiName = apiNameAttribute == null ? string.Empty : apiNameAttribute.Value;
                }

                var useKey = node.Attributes[useKeyVaultAttribute];
                var useKeyValue = useKey == null ? string.Empty : useKey.Value;

                var apiKey = node.Attributes[apiKeyAttribute];
                var apiKeyValue = apiKey == null ? string.Empty : apiKey.Value;
                var hasKeyValue = apiKeyValue.Trim().Length > 0;

                if (useKeyValue.Equals("no", StringComparison.CurrentCultureIgnoreCase) || (hasKeyValue && !IsKeyVaultToken(apiKeyValue)))
                {
                    secretsList.Add($"[ ApiKey: {apiName}, hasKey: {hasKeyValue} ]");
                }
            }
        }

        private static void SearchAppSettings(XmlDocument xmlDoc, string searchTerm, List<string> list)
        {
            var settings = xmlDoc.SelectSingleNode(appSettingsPath);
            if (settings == null)
            {
                return;
            }

            foreach (XmlNode node in settings.ChildNodes)
            {
                if (node.Name != appSettingNode)
                {
                    continue;
                }

                var settingsNode = node.Attributes[appSettingNameAttribute];
                var settingsName = settingsNode == null ? string.Empty : settingsNode.Value.Trim();

                var valueNode = node.Attributes[appSettingValueAttribute];
                var settingsValue = valueNode == null ? string.Empty : valueNode.Value.Trim();

                if (settingsName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) && settingsValue.Length > 0 && !IsKeyVaultToken(settingsValue))
                {
                    list.Add($"Name: {settingsName}");
                }
            }
        }

        //private string GetAttribute(string line, string name)
        //{
        //    int startPos = line.IndexOf(name) + name.Length;
        //    startPos = line.IndexOf("\"", startPos) + 1;
        //    int endPost = line.IndexOf("\"", startPos);

        //    return line.Substring(startPos, endPost - startPos);
        //}

        private static bool IsKeyVaultToken(string value)
        {
            var trimmedValue = value.Trim();
            return trimmedValue.StartsWith(KeyVaultTokenPrefix) && trimmedValue.EndsWith(KeyVaultTokenPostfix);
        }

        #endregion
    }
}
