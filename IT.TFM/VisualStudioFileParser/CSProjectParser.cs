using Parser.Interfaces;
using ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisualStudioFileParser
{
    class CSProjectParser : Parser.ParseXmlFile, IFileParser
    {
        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            buildProperties = data as Dictionary<string, string>;
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
            WriteVSProjectPackageReference(rootNode, xmlPkgReference, file);

            CleanupReferences(file);
        }

        #endregion

        #region Private Methods

        // For the File References, we may have duplicates - some from NuGet package.configs, some from different areas of a Project file
        // We want to look at each set of these, and determine if there are duplicates. If so, we will clean up these references before saving.

        private static void CleanupReferences(FileItem file)
        {
            var packageReferences = file.PackageReferences
                                        .Where(r => r.PackageType == "Project")
                                        .OrderBy(r => r.Id)
                                        .ThenBy(r => r.Version)
                                        .ToArray();

            var referenceCount = packageReferences.Length;
            for (int index = 0; index < referenceCount; index++)
            {
                var reference = packageReferences[index];

                var fileReferences = file.References
                                        .Where(r => r.Equals(reference.Id, StringComparison.InvariantCultureIgnoreCase))
                                        .ToArray();

                foreach (var item in fileReferences)
                {
                    file.References.Remove(item);
                }
            }
        }


        #endregion
    }
}
