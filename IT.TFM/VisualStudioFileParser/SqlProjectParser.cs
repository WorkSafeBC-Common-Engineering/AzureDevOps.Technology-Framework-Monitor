using Parser.Interfaces;
using ProjectData;
using System;

namespace VisualStudioFileParser
{
    class SqlProjectParser : Parser.ParseXmlFile, IFileParser
    {
        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var xmlDoc = GetAsXml(content);
            var rootNode = xmlDoc.DocumentElement;

            if (rootNode == null || !rootNode.Name.Equals(xmlProject, StringComparison.InvariantCultureIgnoreCase))
            {
                InvalidVSProject(file);
                return;
            }

            var value = rootNode.Attributes[xmlAttrToolsVer]?.Value;
            if (value != null)
            {
                file.AddProperty(propertyToolsVersion, value);
            }

            WriteNonNullNodeProperty(rootNode, xmlSchemaVersion, file, propertySchemaVer);
            WriteNonNullNodeProperty(rootNode, xmlProjectVersion, file, propertyProjectVer);
            WriteNonNullNodeProperty(rootNode, xmlDBSchemaProvider, file, propertyDBSchemaVer);
            WriteNonNullNodeProperty(rootNode, xmlOutputType, file, propertyOutputType);
            WriteNonNullNodeProperty(rootNode, xmlTargetFrameworkVersion, file, propertyTargetFWVer);
            WriteNonNullNodeProperty(rootNode, xmlTargetLanguage, file, propertyTargetLanguage);
        }

        #endregion
    }
}
