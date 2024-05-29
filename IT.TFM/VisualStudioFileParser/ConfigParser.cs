using Parser;
using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace VisualStudioFileParser
{
    class ConfigParser : IFileParser
    {
        #region Private Members

        private const string UrlHttps = "\"https:";
        private const string UrlHttp = "\"http:";

        private const string configurationFile = "VisualStudioFileParser.dll.config";
        private const string sectionIgnoreUrls = "ConfigParser-IgnoreUrls";

        private const string secretsParser = "OpenSecrets";

        private static readonly string[] ignoreUrls;

        #endregion

        static ConfigParser()
        {
            ignoreUrls = ConfigurationFileData.ConfigurationSettings.LoadConfigurationValues(configurationFile, sectionIgnoreUrls);
        }

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            foreach (var line in content)
            {
                if (line.Contains(UrlHttps, StringComparison.CurrentCultureIgnoreCase) || line.Contains(UrlHttp, StringComparison.CurrentCultureIgnoreCase))
                {
                    var url = GetUrl(line);
                    if (IgnoreThisUrl(url))
                    {
                        continue;
                    }

                    // ignore this url if it already exists for this file
                    if (file.UrlReferences
                            .Any(u => u.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        continue;
                    }

                    var paths = GetPaths(content, url);
                    foreach (var path in paths)
                    {
                        file.AddUrlReference(url, path);
                    }
                }
            }

            var parser = ContentParserFactory.Get(secretsParser);
            parser.Parse(file, content);
        }

        #endregion

        #region Private Methods

        private static string GetUrl(string line)
        {
            int startPos = line.IndexOf(UrlHttps, StringComparison.InvariantCultureIgnoreCase);
            if (startPos < 0)
            {
                startPos = line.IndexOf(UrlHttp, StringComparison.InvariantCultureIgnoreCase);
            }

            int endPos = line.IndexOf('"', startPos + 1);
            
            return endPos < 0
                ? line[(startPos + 1)..]
                : line.Substring(startPos + 1, endPos - startPos - 1);
        }

        private static bool IgnoreThisUrl(string url)
        {
            return ignoreUrls.Any(u => url.StartsWith(u, StringComparison.InvariantCultureIgnoreCase));
        }

        private static IEnumerable<string> GetPaths(string[] content, string url)
        {
            List<string> paths = [];
            var stack = new Stack<string>();

            var settings = new XmlReaderSettings
            {
                Async = false,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };

            var xmlText = string.Concat(content);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
            {
                using XmlReader reader = XmlReader.Create(stream, settings);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        stack.Pop();
                        continue;
                    }

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        stack.Push(reader.LocalName);

                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Value.Equals(url, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    paths.Add(GetPath(stack, reader.LocalName));
                                }
                            }

                            reader.MoveToElement();
                        }

                        if (reader.IsEmptyElement)
                        {
                            stack.Pop();
                            continue;
                        }

                        if (reader.HasValue && reader.Value.Contains(url))
                        {
                            paths.Add(GetPath(stack, string.Empty));
                        }
                    }
                }
            }

            return paths.AsEnumerable();
        }

        private static string GetPath(Stack<string> stack, string attribute)
        {
            var path = string.Join("/", stack.Reverse());
            if (!string.IsNullOrEmpty(attribute))
            {
                path += $"@{attribute}";
            }

            return path;
        }

        #endregion
    }
}
