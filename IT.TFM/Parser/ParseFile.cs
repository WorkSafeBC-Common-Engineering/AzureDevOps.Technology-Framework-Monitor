using Parser.Interfaces;
using ProjectData;

using System.Collections.Generic;

namespace Parser
{
    public static class ParseFile
    {
        #region Public Methods

        public static void GetProperties(FileItem file, string[] content, Dictionary<string, string> buildProperties)
        {
            IFileParser parser = FileParserFactory.Get(file.FileType);
            parser.Initialize(buildProperties);
            parser.Parse(file, content);
        }

        #endregion
    }
}
