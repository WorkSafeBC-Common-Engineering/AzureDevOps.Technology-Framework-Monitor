using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectData
{
    public class EolVersion
    {
        public string Version { get; set; } = string.Empty;

        public DateOnly? EolDate { get; set; } = DateOnly.MinValue;
    }
}
