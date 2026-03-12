using Parser.Interfaces;

using ProjectData;

using System;
using System.Xml;

namespace VisualStudioFileParser
{
    class SolutionXParser : Parser.ParseXmlFile, IFileParser
    {

        #region Private Members

        private const string xmlSolution = "Solution";
        private const string xmlProperties = "Properties";
        private const string xmlFolder = "Folder";
        private const string xmlConfigurations = "Configurations";
        private const string xmlBuildType = "BuildType";
        private const string xmlPlatform = "Platform";
        private const string xmlAttrName = "Name";
        private const string xmlAttrPath = "Path";
        private const string xmlAttrType = "Type";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var xmlDoc = GetAsXml(content);
            var rootNode = xmlDoc.DocumentElement;

            if (rootNode == null || !rootNode.Name.Equals(xmlSolution, StringComparison.InvariantCultureIgnoreCase))
            {
                file.AddProperty(propertyError, $"Invalid .slnx file - <Solution> must be root node");
                return;
            }

            ParseSolutionProperties(rootNode, file);
            ParseConfigurations(rootNode, file);
            ParseProjects(rootNode, file);
            ParseFolders(rootNode, file);
        }

        #endregion

        #region Private Methods

        private static void ParseSolutionProperties(XmlElement rootNode, FileItem file)
        {
            if (rootNode.SelectSingleNode(xmlProperties) is not XmlElement propertiesNode)
            {
                return;
            }

            var nameNode = propertiesNode.SelectSingleNode(xmlAttrName);
            if (nameNode != null && !string.IsNullOrWhiteSpace(nameNode.InnerText))
            {
                file.AddProperty("SolutionName", nameNode.InnerText);
            }
        }

        private static void ParseConfigurations(XmlElement rootNode, FileItem file)
        {
            if (rootNode.SelectSingleNode(xmlConfigurations) is not XmlElement configurationsNode)
            {
                return;
            }

            if (configurationsNode.SelectSingleNode(xmlBuildType) is XmlElement buildTypeNode)
            {
                var buildType = buildTypeNode.Attributes[xmlAttrName]?.Value;
                if (!string.IsNullOrWhiteSpace(buildType))
                {
                    file.AddProperty("Configuration_BuildType", buildType);
                }
            }

            if (configurationsNode.SelectSingleNode(xmlPlatform) is XmlElement platformNode)
            {
                var platform = platformNode.Attributes[xmlAttrName]?.Value;
                if (!string.IsNullOrWhiteSpace(platform))
                {
                    file.AddProperty("Configuration_Platform", platform);
                }
            }
        }

        private static void ParseProjects(XmlElement rootNode, FileItem file)
        {
            var projectNodes = rootNode.SelectNodes($"//{xmlProject}");
            if (projectNodes == null)
            {
                return;
            }

            foreach (XmlElement projectNode in projectNodes)
            {
                var pathAttr = projectNode.Attributes[xmlAttrPath]?.Value;
                if (!string.IsNullOrWhiteSpace(pathAttr))
                {
                    file.AddReference(pathAttr);
                }

                var typeAttr = projectNode.Attributes[xmlAttrType]?.Value;
                if (!string.IsNullOrWhiteSpace(typeAttr))
                {
                    file.AddProperty($"ProjectType_{pathAttr}", typeAttr);
                }
            }
        }

        private static void ParseFolders(XmlElement rootNode, FileItem file)
        {
            var folderNodes = rootNode.SelectNodes($"//{xmlFolder}");
            if (folderNodes == null)
            {
                return;
            }

            var folderCount = 0;
            foreach (XmlElement folderNode in folderNodes)
            {
                var nameAttr = folderNode.Attributes[xmlAttrName]?.Value;
                if (!string.IsNullOrWhiteSpace(nameAttr))
                {
                    file.AddProperty($"SolutionFolder_{folderCount}", nameAttr);
                    folderCount++;
                }
            }

            if (folderCount > 0)
            {
                file.AddProperty("SolutionFolderCount", folderCount.ToString());
            }
        }

        #endregion
    }
}
