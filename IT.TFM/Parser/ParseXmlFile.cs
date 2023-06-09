﻿using ConfigurationFileData;
using ProjectData;
using System.Linq;
using System.Xml;

namespace Parser
{
    public abstract class ParseXmlFile
    {
        #region Private Members

        private const string configurationFile = "Parser.dll.config";
        private const string sectionIgnoreFileRefs = "FileReferences-Ignore";

        private static readonly string[] fileRefPunctuation = { " ", ",", "." };
        private static readonly string[] ignoreFileRefs;

        #endregion

        static ParseXmlFile()
        {
            ignoreFileRefs = ConfigurationSettings.LoadConfigurationValues(configurationFile, sectionIgnoreFileRefs);
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

        protected const string xmlAttrToolsVer = @"ToolsVersion";
        protected const string xmlAttrSdk = @"Sdk";
        protected const string xmlAttrInclude = @"Include";

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

        protected XmlNamespaceManager mgr;

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

        protected void InvalidVSProject(FileItem file)
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
                    file.AddReference(attribute);
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

        protected string NamespacePath(string path)
        {
            var fields = path.Split(new char[] { '/' });
            var newFields = fields.Select(f => "msbld:" + f);
            return string.Join("/", newFields);
        }

        protected bool ValidReference(string reference)
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
        #endregion
    }
}
