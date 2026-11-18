# 04.03-specialized-parsers: Add .NET 11 targets to specialized parser libraries

## Objective
Multi-target parser and configuration libraries that consume the core services.

## Scope
- `PythonParser\PythonFileParser.csproj`
- `YamlFileParser\YamlFileParser.csproj`
- `NuGetFileParser\NuGetFileParser.csproj`
- `NpmFileParser\NpmFileParser.csproj`
- `VisualStudioFileParser\VisualStudioFileParser.csproj`
- `EndOfLifeConfigFile\EndOfLifeConfigFile.csproj`

## Research Findings
- Confirmed all six projects currently target `net10.0` and are SDK-style class libraries. They directly reference the core libraries already multi-targeting `net10.0;net11.0`; no source edits are required unless compilation demonstrates otherwise.
- `PythonFileParser` references `Parser`, `ProjectData`, and `Storage`; `YamlFileParser` references `Parser`; `NuGetFileParser` references `Parser` and `ProjectData`; `NpmFileParser` references `Parser` and `ProjectData`; `VisualStudioFileParser` references `ConfigurationFileData`, `Parser`, and `ProjectData`; `EndOfLifeConfigFile` references `ConfigurationFileData`, `Parser`, and `ProjectData`.
- Existing package references are `Newtonsoft.Json` 13.0.4 in `NpmFileParser` and `System.Configuration.ConfigurationManager` 11.0.0 in `VisualStudioFileParser`; preserve them unless restore/build evidence requires changes. The aggregate assessment suggests 10.0.15 for ConfigurationManager, but that is not itself proof of a compatibility failure.
- The Visual Studio parser assessment reports zero binary/source incompatible APIs and one behavioral-change flag; there is no corresponding compiler issue, so retain current behavior and include the targeted build (and available Python parser tests) in validation.
- `PythonVersionUnitTests` is under `IT.TFM\Tests\UnitTests\PythonVersionUnitTests`; its final target consolidation is planned for task 05, while this batch keeps it on its current target and checks it against the parser libraries' .NET 10 assets.

## Steps
1. Review per-project assessment/package notes and relevant parser/configuration behavior.
2. Add `net11.0` while retaining `net10.0`.
3. Restore and build both targets; address only evidenced API or package issues.

## Done when
All six projects build for both TFMs and their project references resolve.
