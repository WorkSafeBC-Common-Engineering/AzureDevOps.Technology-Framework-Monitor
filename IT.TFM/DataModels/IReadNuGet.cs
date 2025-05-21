using ProjectData;

using System.Collections.Generic;

namespace RepoScan.DataModels
{
    public interface IReadNuGet
    {
        IEnumerable<NuGetFeed> ListFeeds();
    }
}
