using Parser.Interfaces;
using ProjectData;

namespace Parser
{
    public static class ParseFile
    {
        #region Public Methods

        public static void GetProperties(FileItem file, string[] content)
        {
            IFileParser parser = FileParserFactory.Get(file.FileType);
            parser.Parse(file, content);
        }

        #endregion
    }
}
