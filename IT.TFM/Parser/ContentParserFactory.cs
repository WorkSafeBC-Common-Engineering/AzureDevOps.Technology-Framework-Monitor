using Parser.Interfaces;
using ProjectData;

namespace Parser
{
    public static class ContentParserFactory
    {
        #region Factory Methods

        public static IContentParser Get(string itemType)
        {
            return ServiceProviderAccessor.GetRequiredKeyedService<IContentParser>(itemType);
        }

        #endregion
    }
}
