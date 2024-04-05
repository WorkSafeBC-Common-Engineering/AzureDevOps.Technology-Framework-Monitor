namespace Parser.Interfaces
{
    public interface IContentParser
    {
        void Parse(ProjectData.FileItem file, string[] content);
    }
}
