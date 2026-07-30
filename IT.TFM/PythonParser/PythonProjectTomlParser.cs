using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace PythonFileParser
{
    public class PythonProjectTomlParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersionProjectToml";
        private const string majorVersionKey = "PythonMajorVersionProjectToml";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var cleanContent = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                if (string.IsNullOrEmpty(content[i]))
                { 
                    continue;
                }

                if (content[i].Contains("python_version") || content[i].Contains("PYTHON_VERSION"))
                {
                    cleanContent.Append(content[i]);
                    break;
                }

                if (content[i].Contains("requires-python"))
                {
                    cleanContent.Append(content[i]);
                    break;
                }
            }
            ParseVersionFile(file, cleanContent.ToString());
        }

        #endregion

        #region Private Methods

        private static void ParseVersionFile(FileItem file, string cleanContent)
        {
            if (!file.Path.Contains("pyproject.toml"))
            {  
               return;
            }

            if (string.IsNullOrEmpty(cleanContent) || !cleanContent.Contains(" = "))
            {
                return;
            }

            var versionDetail = cleanContent.Split(" = ")[1].Trim();

            //Clear quotes from the versionDetail string, if they exist
            versionDetail = versionDetail.Replace("\"", "");
            versionDetail = versionDetail.Replace("'", "");

            var version = versionDetail.Contains(',') ? versionDetail.Split(",")[0] : versionDetail;

            //Remove conditional operators from the version string, if they exist
            version = version.Replace("=", "");
            version = version.Replace(">", "");
            version = version.Replace("&gt;", "");

            file.AddProperty(versionKey, version);

            //This covers versions that have the '-slim', or other suffixes
            version = version.Contains('-') ? version.Split("-")[0] : version;

            file.AddProperty(majorVersionKey, version);
        }

        #endregion
    }
}