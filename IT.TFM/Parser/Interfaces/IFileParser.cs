using ProjectData;

namespace Parser.Interfaces
{
    public interface IFileParser
    {
        void Parse(FileItem file, string[] content);
    }
}
