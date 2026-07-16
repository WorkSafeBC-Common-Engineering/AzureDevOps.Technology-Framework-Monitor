using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace PythonFileParser
{
    public class PythonDockerParser : IFileParser
    {
        #region Private Members

        private const string versionKey = "DockerfilePythonVersion";

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
                if (content[i].Contains("FROM python:"))
                    cleanContent += content[i];
            }
            ParseVersionFile(file, cleanContent);
        }

        #endregion

        #region Private Methods

        private static void ParseVersionFile(FileItem file, string cleanContent)
        {
            if (!file.Path.Contains("Dockerfile"))
                return;

            var version = cleanContent.Split(":")[1].Trim();

            //This covers the 'AS base', or similar specifiers that may be in the version string
            version = version.Contains(' ') ? version.Split(" ")[0] : version;

            //This covers versions that have the '-slim', or other suffixes
            version = version.Contains('-') ? version.Split("-")[0] : version;

            file.AddProperty(versionKey, version);
        }

        #endregion
    }
}