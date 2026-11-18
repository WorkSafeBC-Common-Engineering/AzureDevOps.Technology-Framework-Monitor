using ProjectData;
using ProjectData.Interfaces;

namespace Parser
{
    public class EndOfLifeCheckerFactory
    {
        #region Factory Methods

        public static IEndOfLifeChecker Get()
        {
            return ServiceProviderAccessor.GetRequiredService<IEndOfLifeChecker>();
        }

        #endregion
    }
}
