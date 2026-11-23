using System;

using ConfigurationFileData;

namespace Parser
{
    public sealed class XmlParserSettings
    {
        private readonly Lazy<string[]> ignoredFileReferences;

        public XmlParserSettings(ConfigurationSettings configurationSettings)
        {
            ArgumentNullException.ThrowIfNull(configurationSettings);
            ignoredFileReferences = new Lazy<string[]>(() => configurationSettings.LoadConfigurationValues("Parser.dll.config", "FileReferences-Ignore"));
        }

        public string[] IgnoredFileReferences => ignoredFileReferences.Value;
    }
}
