using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace PythonFileParser
{
    public class PythonVersionParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "PythonVersion";
        private const string majorVersionKey = "PythonMajorVersion";

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

                cleanContent.Append(content[i]);
            }
            ParseVersionFile(file, cleanContent.ToString());
        }

        #endregion

        #region Private Methods

        private static void ParseVersionFile(FileItem file, string cleanContent)
        {
            if (!file.Path.Contains(".python-version"))
            {
                return;
            }

            var version = cleanContent.Trim();

            file.AddProperty(versionKey, version);

            //This covers versions that have the '-slim', or other suffixes
            version = version.Contains('-') ? version.Split("-")[0] : version;

            file.AddProperty(majorVersionKey, version);
        }

        #endregion
    }
}