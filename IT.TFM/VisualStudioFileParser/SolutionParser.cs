using Parser.Interfaces;
using ProjectData;

namespace VisualStudioFileParser
{
    class SolutionParser : IFileParser
    {
        #region Private Members

        private const string propertyFileFormat = "FileFormat";
        private const string propertyVSVersion = "VisualStudioVersion";
        private const string propertyMinVSVersion = "MinimumVisualStudioVersion";

        private const int slnLineFileFormat = 0;
        private const int slnLineVSVersion = 2;
        private const int slnLineMinVSVersion = 3;
        private const int slnLineMax = slnLineMinVSVersion;

        #endregion

        #region IFileParser Implementation

        void IFileParser.Parse(FileItem file, string[] content)
        {
            int index = 0;
            foreach (var line in content)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    switch (index)
                    {
                        case slnLineFileFormat:
                            AddSolutionProperty(file, line, propertyFileFormat);
                            break;

                        case slnLineVSVersion:
                            AddSolutionProperty(file, line, propertyVSVersion);
                            break;

                        case slnLineMinVSVersion:
                            AddSolutionProperty(file, line, propertyMinVSVersion);
                            break;
                    }

                    if (index >= slnLineMax)
                    {
                        break;
                    }

                    index++;
                }
            }
        }

        #endregion

        #region Private Methods

        private static void AddSolutionProperty(FileItem file, string line, string property)
        {
            var cleanLine = line.Trim();
            var position = cleanLine.LastIndexOf(' ');
            if (position > -1 && position < cleanLine.Length - 1)
            {
                file.AddProperty(property, cleanLine[(position + 1)..]);
            }
        }

        #endregion

    }
}
