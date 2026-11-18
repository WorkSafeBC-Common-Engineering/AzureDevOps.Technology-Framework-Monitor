# 05-library-consolidation: Consolidate libraries and upgrade Python parser tests

## Objective
Remove the temporary .NET 10 target from shared libraries, move the Python parser test project to .NET 11, and preserve non-TFM behavior.

## Research Findings
- Confirmed 28 SDK-style library projects currently target `net11.0;net10.0`: `AzureDevOps`, `AzureDevOpsScannerFramework`, `CodeFileFiltering`, `ConfidentialSettings`, `ConfigurationFileData`, `ContentFilter`, `DataModels`, `DefaultMetricsScanner`, `EndOfLifeConfigFile`, `FileLocator`, `FilterConfigurationFromConfigFile`, `NpmFileParser`, `NuGetFileParser`, `NuGetScanner`, `OpenSecretsDetection`, `ParameterSettings`, `Parser`, `ProjectDataFramework`, `ProjectScannerFramework`, `ProjectScannerSaveToSqlServer`, `PythonParser`, `ReportsRefresh`, `RepoScan.Storage.SqlServer`, `RuntimeMetricsScanner`, `Storage`, `VisualStudioFileParser`, `VisualStudioMetricsScanner`, and `YamlFileParser`.
- `PythonVersionUnitTests.csproj` is the only test project and currently targets `net10.0`; it references `PythonFileParser` and `YamlFileParser`. `TfmExport` and `TfmScanWithToken` already target only `net11.0`. `ProjectScannerDB.sqlproj` remains on its SQL-specific target and is excluded from NuGet TFM management.
- Target properties are declared directly in the project files. No `Directory.Build.props` exists, and the dependency inspection found no imported local target overrides for the app or test project.
- Dependency levels were recomputed from all project references. Execute leaf-to-root: tier 0: `Parameters`, `ConfidentialSettings`, `ProjectData`; tier 1: `RepoScan.DataModels`, `ConfigurationFileData`, `NuGetScanner`, `DefaultMetricsScanner`, `Storage`, `ReportsRefresh`, `ContentFilter`, `AzureDevOps`, `VisualStudioMetricsScanner`, `ProjectScannerSaveToSqlServer`, `FileFiltering`; tier 2: `RepoScan.Storage.SqlServer`, `Parser`, `FilterConfigurationFromConfigFile`; tier 3: `EndOfLifeConfigFile`, `YamlFileParser`, `PythonFileParser`, `VisualStudioFileParser`, `NpmFileParser`, `ProjectScanner`, `NuGetFileParser`, `OpenSecretsDetection`; tier 4: `AzureDevOpsScannerFramework`, `RepoScan.FileLocator`, `RuntimeMetricsScanner`, and `PythonVersionUnitTests`. The application is already net11-only.
- Assessment summaries were consulted for all 29 scoped projects. They confirm SDK-style projects and a required TFM change; some carry unrelated API/package findings, while this task changes no API or package references. Existing full-solution builds had already validated the net11 graph.
- No .NET-version-specific conditional compilation or target-framework-conditioned project items were found. The only preprocessor block is `#if DEBUG` in `TfmScanWithToken\Program.cs`; preserve it.
- Defer Central Package Management until after this migration: “All projects are now SDK-style and on a single TFM — CPM can be added cleanly without VersionOverride friction.” This applies to .NET package-consuming projects; the SQL database project remains outside NuGet TFM management.

## Execution Plan
Break down by dependency tier and execute in order. Change `TargetFrameworks` to singular `TargetFramework` with `net11.0` for each scoped library, and change the test project's `TargetFramework` to `net11.0`. Build each tier before proceeding; after the test tier, run the Python parser tests. Do not remove any conditional compilation unless a TFM-specific branch is found and proven obsolete.

## Done when
All .NET application, library, and test projects target only `net11.0`; affected tests pass; no required non-TFM conditional behavior has been removed; the deferred CPM recommendation is documented.
