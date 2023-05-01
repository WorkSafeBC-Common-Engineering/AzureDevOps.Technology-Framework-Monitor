using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Interfaces
{
    public interface IContentParser
    {
        void Parse(ProjectData.FileItem file, string[] content);
    }
}
