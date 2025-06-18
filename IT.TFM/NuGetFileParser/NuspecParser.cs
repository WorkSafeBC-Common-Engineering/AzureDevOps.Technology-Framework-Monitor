using Parser.Interfaces;

using ProjectData;

using System.Xml;

namespace NuGetFileParser
{
    public class NuspecParser : Parser.ParseXmlFile, IFileParser
    {
        #region Private Members
        
        private const string xmlNuspecPackage = "package";
        private const string xmlMetadata = "metadata";
        private const string xmlId = "id";

        #endregion

        #region IFileParser Implentation

        void IFileParser.Initialize(object data)
        {
            //no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var xmlDoc = GetAsXml(content);
            var rootNode = xmlDoc.DocumentElement;

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("ns", rootNode.NamespaceURI);

            var idNode = rootNode.SelectSingleNode("//ns:package/ns:metadata/ns:id", nsmgr);
            if (idNode == null)
            {
                return;
            }

            var id = idNode.InnerText;
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            file.AddProperty("Id", id);
        }

        #endregion
    }
}
