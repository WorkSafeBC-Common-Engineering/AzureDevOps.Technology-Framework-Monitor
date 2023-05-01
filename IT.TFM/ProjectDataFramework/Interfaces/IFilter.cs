using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData.Interfaces
{
    public interface IFilter
    {
        void Initialize(string data);

        string ColumnName { get; }

        bool IsMatch(string data);
    }
}
