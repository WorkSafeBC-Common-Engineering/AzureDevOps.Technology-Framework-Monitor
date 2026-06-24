using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace NpmFileParser
{
    public class NvmrcParser : IFileParser
    {
        #region Private Members

        private const string prefix = "v";
        private const string versionKey = "Version";
        private const string majorVersionKey = "MajorVersion";

        #endregion

        #region IFileParse Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var version = content[0].Trim();
            file.AddProperty(versionKey, version);

            if (version.StartsWith(prefix))
            {
                version = version[prefix.Length..];
            }

            var majorVersion = version.Split('.')[0];
            if (int.TryParse(majorVersion, out _))
            {
                file.AddProperty(majorVersionKey, majorVersion);
            }            
        }

        #endregion
    }
}
