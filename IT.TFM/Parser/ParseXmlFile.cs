﻿using ProjectData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Parser
{
    public abstract class ParseXmlFile
    {
        #region Private Members

        private const string configurationFile = "Parser.dll.config";
        private const string sectionIgnoreFileRefs = "FileReferences-Ignore";

        private static readonly string[] fileRefPunctuation = [" ", ",", "."];
        private static readonly string[] ignoreFileRefs;

        protected Dictionary<string, string> buildProperties;

        #endregion

        static ParseXmlFile()
        {
            ignoreFileRefs = ConfigurationFileData.ConfigurationSettings.LoadConfigurationValues(configurationFile, sectionIgnoreFileRefs);
        }

        #region Protected Members

        protected const string xmlProject = @"Project";
        protected const string xmlSchemaVersion = @"PropertyGroup/SchemaVersion";
        protected const string xmlProjectVersion = @"PropertyGroup/ProjectVersion";
        protected const string xmlDBSchemaProvider = @"PropertyGroup/DSP";
        protected const string xmlOutputType = @"PropertyGroup/OutputType";
        protected const string xmlTargetFrameworkVersion = @"PropertyGroup/TargetFrameworkVersion";
        protected const string xmlTargetFramework = @"PropertyGroup/TargetFramework";
        protected const string xmlTargetLanguage = @"PropertyGroup/TargetLanguage";
        protected const string xmlAndroidApp = @"PropertyGroup/AndroidApplication";
        protected const string xmlAzureFunction = @"PropertyGroup/AzureFunctionsVersion";
        protected const string xmliOSApp = "ItemGroup/Reference[@Include=\"Xamarin.iOS\"]";
        protected const string xmlReferences = @"ItemGroup/Reference";
        protected const string xmlPkgReference = @"ItemGroup/PackageReference";
        protected const string xmlHintPath = @"HintPath";

        protected const string xmlAttrToolsVer = @"ToolsVersion";
        protected const string xmlAttrSdk = @"Sdk";
        protected const string xmlAttrInclude = @"Include";
        protected const string xmlAttrPkgVersion = @"Version";

        protected const string propertyError = "Error";
        protected const string propertyToolsVersion = "ToolsVersion";
        protected const string propertySchemaVer = "SchemaVersion";
        protected const string propertyProjectVer = "ProjectVersion";
        protected const string propertyDBSchemaVer = "DBSchemaProvider";
        protected const string propertyOutputType = "OutputType";
        protected const string propertyTargetFWVer = "TargetFrameworkVersion";
        protected const string propertyTargetFW = "TargetFramework";
        protected const string propertyTargetLanguage = "TargetLanguage";
        protected const string propertySdk = @"Sdk";
        protected const string propertyIsAndroid = @"AndroidApp";
        protected const string propertyIsiOS = @"iOSApp";
        protected const string propertyAzureFunction = @"AzureFunction";

        private const string PackageProjectType = "Project";

        protected XmlNamespaceManager mgr;
        private static readonly char[] namespaceSeparator = ['/'];
        private static readonly char[] pkgSeparator = [','];
        private static readonly char[] attributeValueSeparator = ['='];
        private static readonly char[] pathSeparator = ['\\', '/'];
        private static readonly char[] hintPathVersionSeparator = ['.'];

        #endregion

        #region Protected Methods

        protected XmlDocument GetAsXml(string[] content)
        {
            var data = string.Concat(content);
            var xml = new XmlDocument();
            xml.LoadXml(data);

            var rootNode = xml.DocumentElement;
            var attr = rootNode.Attributes["xmlns"];
            if (attr == null)
            {
                mgr = null;
            }
            else
            {
                var ns = attr.Value;
                mgr = new XmlNamespaceManager(xml.NameTable);
                mgr.AddNamespace("msbld", ns);
            }

            return xml;
        }

        protected static void InvalidVSProject(FileItem file)
        {
            file.AddProperty(propertyError, $"Invalid project file - <Project> must be root node");
        }

        protected void WriteNonNullNodeProperty(XmlElement rootNode, string xPath, FileItem file, string property)
        {
            var node = SingleNode(rootNode, xPath);
            if (node != null)
            {
                file.AddProperty(property, node.InnerText);
            }
        }

        protected void WriteTrueNodeProperty(XmlElement rootNode, string xPath, FileItem file, string property, string trueValue, string falseValue = "")
        {
            var node = SingleNode(rootNode, xPath);
            if (node != null  && bool.TryParse(node.InnerText, out bool value))
            {
                file.AddProperty(property, value ? trueValue : falseValue);
                return;
            }

            file.AddProperty(property, falseValue);
        }

        protected void WriteIfExistsProperty(XmlElement rootNode, string xPath, FileItem file, string property, string trueValue, string falseValue = "")
        {
            var node = SingleNode(rootNode, xPath);
            file.AddProperty(property, node == null ? falseValue : trueValue);
        }

        protected void WriteVSProjectReferences(XmlElement rootNode, string xPath, FileItem file)
        {
            var nodeList = AllNodes(rootNode, xPath);
            foreach (XmlNode node in nodeList)
            {

                var attribute = node.Attributes[xmlAttrInclude]?.Value;
                if (ValidReference(attribute))
                {
                    var pkgFields = attribute.Split(pkgSeparator, System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

                    var pkgName = pkgFields.First();

                    var versionAttribute = pkgFields.FirstOrDefault(f => f.StartsWith(xmlAttrPkgVersion));
                    if (versionAttribute != null)
                    {
                        var versionFields = versionAttribute.Split(attributeValueSeparator, System.StringSplitOptions.TrimEntries);
                        var versionValue = versionFields.Length == 2 ? versionFields[1] : null;

                        if (versionValue != null)
                        {
                            file.AddPackageReference(PackageProjectType, pkgName, versionValue, null, null);
                            continue;
                        }
                    }

                    else
                    {
                        // One last chance - can we get the version number based on the Hint Path (if there is one)
                        var version = GetHintPathVersion(node);
                        if (version != null)
                        {
                            file.AddPackageReference(PackageProjectType, pkgName, version, null, null);
                        }
                    }

                    file.AddReference(attribute);
                }
            }
        }

        protected void WriteVSProjectPackageReference(XmlElement rootNode, string xPath, FileItem file)
        {
            var nodeList = AllNodes(rootNode, xPath);
            foreach (XmlNode node in nodeList)
            {
                var attribute = node.Attributes[xmlAttrInclude]?.Value;
                string versionAttribute = string.Empty;

                if (ValidReference(attribute))
                {
                    foreach (XmlAttribute attributeValue in node.Attributes)
                    {                         
                        if (attributeValue.Name.Equals(xmlAttrPkgVersion, System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            versionAttribute = node.Attributes[xmlAttrPkgVersion]?.Value;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(versionAttribute))
                    {
                        versionAttribute = string.Empty;
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.Name.Equals("Version", System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                versionAttribute = childNode.InnerText;
                                break;
                            }
                        }
                    }
                    else if (versionAttribute.StartsWith('$'))
                    {
                        versionAttribute = versionAttribute.Replace("$(", string.Empty).Replace(")", string.Empty);
                        // find the entry from the Directory.Build.props file - if not found then leave this blank.
                        if (buildProperties != null && buildProperties.TryGetValue(versionAttribute, out string value))
                        {
                            versionAttribute = value;
                        }
                        else
                        {
                            versionAttribute = string.Empty;
                        }
                    }

                    file.AddPackageReference(PackageProjectType, attribute, versionAttribute, null, null);
                }
            }
        }

        protected XmlNode SingleNode(XmlElement node, string path)
        {
            return mgr == null
                ? node.SelectSingleNode(path)
                : node.SelectSingleNode(NamespacePath(path), mgr);
        }

        protected XmlNodeList AllNodes(XmlElement node, string path)
        {
            return mgr == null
                ? node.SelectNodes(path)
                : node.SelectNodes(NamespacePath(path), mgr);

        }

        protected static string NamespacePath(string path)
        {
            var fields = path.Split(namespaceSeparator);
            var newFields = fields.Select(f => "msbld:" + f);
            return string.Join("/", newFields);
        }

        protected static bool ValidReference(string reference)
        {
            if (reference == null)
            {
                return false;
            }

            var refValue = reference.Trim();

            foreach (var fileRef in ignoreFileRefs)
            {
                if (refValue.Equals(fileRef, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                foreach (var punctuation in fileRefPunctuation)
                {
                    if (refValue.StartsWith(fileRef + punctuation))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Using the Hint Path, try and parse out a version number.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Null if no version found, else returns the version # as string.</returns>
        protected static string GetHintPathVersion(XmlNode node)
        {
            var childNodes = node.ChildNodes;
            foreach (XmlNode childNode in childNodes)
            {
                if (!childNode.LocalName.Equals(xmlHintPath, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var path = childNode.InnerText;

                if (Parameters.Settings.ExtendedLogging)
                {
                    Console.WriteLine($"\t>>> Using HintPath: {path}");
                }

                var fields = path.Split(pathSeparator, System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

                foreach (var field in fields)
                {
                    if (field == "." || field == ".." || field == "packages")
                    {
                        continue;
                    }
                    
                    return ExtractVersionFromHintPathSegment(field);
                }                
            }

            return null;
        }

        private static string ExtractVersionFromHintPathSegment(string segment)
        {
            string version = null;

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"\t>>> Getting Package Version from HintPath segment: {segment}");
            }

            var fields = segment.Split(hintPathVersionSeparator, System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                if (int.TryParse(field, out _))
                {
                    // Start of version number, just add this and the remaining fields as the version.
                    version = string.Join(hintPathVersionSeparator[0], fields[index..]);
                    break;
                }
            }

            if (Parameters.Settings.ExtendedLogging)
            {
                Console.WriteLine($"\t>>> Getting Package Version from HintPath, version = {version}");
            }
            return version;
        }

        #endregion
    }
}
