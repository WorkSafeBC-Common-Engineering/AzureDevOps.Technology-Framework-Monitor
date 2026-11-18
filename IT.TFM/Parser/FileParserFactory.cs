using Parser.Interfaces;
using ProjectData;

namespace Parser
{
    public static class FileParserFactory
    {
        #region Factory Methods

        public static IFileParser Get(FileItemType itemType)
        {
            return ServiceProviderAccessor.GetRequiredKeyedService<IFileParser>(itemType.ToString());
        }

        #endregion
    }
}
