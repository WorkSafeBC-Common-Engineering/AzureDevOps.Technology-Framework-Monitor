using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace PythonFileParser
{
    public class PythonSetupPyParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "SetupPyPythonVersion";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var cleanContent = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (String.IsNullOrEmpty(content[i]))
                    continue;
                if (content[i].Contains("python_requires"))
                {
                    cleanContent += content[i];
                    break;
                }
            }
            ParseVersionFile(file, cleanContent);
        }

        #endregion

        #region Private Methods

        private static void ParseVersionFile(FileItem file, string cleanContent)
        {
            if (!file.Path.Contains("setup.py"))
                return;

            if (string.IsNullOrEmpty(cleanContent) || !cleanContent.Contains(">="))
                return;

            var version = cleanContent.Split(">=")[1].Trim();

            //Clear quotes and comma from the version string, if they exist
            version = version.Replace("\"", "");
            version = version.Replace("'", "");
            version = version.Replace(",", "");

            file.AddProperty(versionKey, version);
        }

        #endregion
    }
}