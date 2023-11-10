using Parser.Interfaces;
using ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NuGetFileParser
{
    public class ProjectPackagesParser : Parser.ParseXmlFile, IFileParser
    {
        #region Private Members

        private const string NuGetPackageType = "NuGet";

        private const string xmlNuGetPackage = "packages";

        private const string nodePath = "package";

        private const string attributeId = "id";
        private const string attributeVersion = "version";
        private const string attributeTargetFramework = "targetFramework";

        #endregion

        #region IFileParser Implentation

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var xmlDoc = GetAsXml(content);
            var rootNode = xmlDoc.DocumentElement;

            if (rootNode == null || !rootNode.Name.Equals(xmlNuGetPackage, StringComparison.InvariantCultureIgnoreCase))
            {
                InvalidNuGetPackages(file);
                return;
            }

            var nodes = AllNodes(rootNode, nodePath);
            foreach (XmlNode node in nodes)
            {
                var id = node.Attributes[attributeId]?.Value;

                if (!ValidReference(id))
                {
                    continue;
                }

                var version = node.Attributes[attributeVersion]?.Value;
                var framework = node.Attributes[attributeTargetFramework]?.Value;

                file.AddPackageReference(NuGetPackageType, id, version, framework, string.Empty);
            }
        }

        #endregion

        #region Private Methods

        private static void InvalidNuGetPackages(FileItem file)
        {
            file.AddProperty(propertyError, $"Invalid NuGet packages file - <packages> must be root node");
        }

        #endregion
    }
}
