using ProjectData;

namespace Parser.Interfaces
{
    public interface IFileParser
    {
        void Initialize(object data);

        void Parse(FileItem file, string[] content);
    }
}
