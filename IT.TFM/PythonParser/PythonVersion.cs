using System;
using System.Collections.Generic;
using System.Text;

namespace PythonFileParser
{
    internal class PythonVersion
    {
        public string Version { get; set; } = string.Empty;

        public DateOnly EolDate { get; set; } = DateOnly.MinValue;
    }
}
