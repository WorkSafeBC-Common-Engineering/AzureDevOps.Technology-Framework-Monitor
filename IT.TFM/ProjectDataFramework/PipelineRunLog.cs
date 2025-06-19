using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData
{
    public class PipelineRunLog
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastChangedOn { get; set; }
    }
}
