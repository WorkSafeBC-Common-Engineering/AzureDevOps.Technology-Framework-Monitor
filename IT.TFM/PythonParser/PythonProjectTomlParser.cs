using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Text;

namespace PythonParser
{
    public class PythonProjectTomlParser : IFileParser
    {
        #region IFileParser Members

        void IFileParser.Initialize(object data)
        {
            throw new NotImplementedException();
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}