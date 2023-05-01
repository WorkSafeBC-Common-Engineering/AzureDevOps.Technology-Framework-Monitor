using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IEndOfLifeChecker
    {
        bool IsEndOfLife(ComponentTypeEnum componentType, string version);

        DateTime? GetEndOfLifeDate(ComponentTypeEnum componentType, string version);
    }
}
