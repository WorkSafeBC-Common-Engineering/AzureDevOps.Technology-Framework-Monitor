using Parser.Interfaces;
using ProjectData;
using System;

namespace VisualStudioFileParser
{
    class CSProjectParser : Parser.ParseXmlFile, IFileParser
    {
        #region IFileParser Implementation

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

            value = rootNode.Attributes[xmlAttrSdk]?.Value;
            if (value != null)
            {
                file.AddProperty(propertySdk, value);
            }

            WriteNonNullNodeProperty(rootNode, xmlOutputType, file, propertyOutputType);
            WriteNonNullNodeProperty(rootNode, xmlTargetFrameworkVersion, file, propertyTargetFWVer);
            WriteNonNullNodeProperty(rootNode, xmlTargetFramework, file, propertyTargetFW);
            WriteNonNullNodeProperty(rootNode, xmlAzureFunction, file, propertyAzureFunction);

            WriteTrueNodeProperty(rootNode, xmlAndroidApp, file, propertyIsAndroid, "Yes");
            WriteIfExistsProperty(rootNode, xmliOSApp, file, propertyIsiOS, "Yes");

            WriteVSProjectReferences(rootNode, xmlReferences, file);
            WriteVSProjectReferences(rootNode, xmlPkgReference, file);
        }

        #endregion
    }
}
