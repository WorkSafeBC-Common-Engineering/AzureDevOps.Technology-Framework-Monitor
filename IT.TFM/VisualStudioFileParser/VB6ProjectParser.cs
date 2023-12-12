using Parser.Interfaces;
using ProjectData;

namespace VisualStudioFileParser
{
    class VB6ProjectParser : IFileParser
    {
        #region Private Members

        private const string propertyType = "Type";
        private const string propertyExeName32 = "ExeName32";
        private const string propertyMajorVersion = "MajorVersion";
        private const string propertyMinorVersion = "MinorVersion";
        private const string propertyRevisionVersion = "RevisionVersion";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            foreach (var line in content)
            {
                var position = line.IndexOf('=');
                if (position > 0)
                {
                    var key = line.Substring(0, position);
                    var value = StripQuotes(line.Substring(position + 1));

                    switch (key.ToLower())
                    {
                        case "type":
                            file.AddProperty(propertyType, value);
                            break;

                        case "reference":
                            file.AddReference(value);
                            break;

                        case "exename32":
                            file.AddProperty(propertyExeName32, value);
                            break;

                        case "majorver":
                            file.AddProperty(propertyMajorVersion, value);
                            break;

                        case "minorver":
                            file.AddProperty(propertyMinorVersion, value);
                            break;

                        case "revisionver":
                            file.AddProperty(propertyRevisionVersion, value);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private static string StripQuotes(string text)
        {
            if (text.StartsWith("'") || text.StartsWith("\""))
            {
                text = text.Substring(1);
            }

            if (text.EndsWith("'") || text.EndsWith("\""))
            {
                text = text.Substring(0, text.Length - 1);
            }

            return text;
        }

        #endregion
    }
}
