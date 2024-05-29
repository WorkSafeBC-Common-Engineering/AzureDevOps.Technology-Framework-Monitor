using System;

namespace ProjectData.Interfaces
{
    public interface IEndOfLifeChecker
    {
        bool IsEndOfLife(ComponentTypeEnum componentType, string version);

        DateTime? GetEndOfLifeDate(ComponentTypeEnum componentType, string version);
    }
}
