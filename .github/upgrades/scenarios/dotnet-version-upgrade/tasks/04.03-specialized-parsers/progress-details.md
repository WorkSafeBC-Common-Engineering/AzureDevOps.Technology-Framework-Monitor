# 04.03-specialized-parsers Progress

- Added `net11.0` while retaining `net10.0` in PythonFileParser, YamlFileParser, NuGetFileParser, NpmFileParser, VisualStudioFileParser, and EndOfLifeConfigFile.
- Preserved existing package versions, including `Newtonsoft.Json` 13.0.4 and `System.Configuration.ConfigurationManager` 11.0.0.
- The initial IDE build exposed a malformed `ImplicitUsings` element in YamlFileParser and stale project restore state. Corrected the XML element and forced restore of the changed projects and PythonVersionUnitTests.
- Validation: all six parser/configuration projects built for both .NET 10 and .NET 11 with zero warnings and zero errors.
- Validation: `PythonVersionUnitTests` ran on its temporary .NET 10 target; all 65 tests passed.
