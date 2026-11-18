# 05-library-consolidation: Consolidate libraries and upgrade Python parser tests

## Objective
Remove the temporary .NET 10 target from shared libraries, move the Python parser test project to .NET 11, and preserve non-TFM behavior.

## Research Findings
- The scope was 28 SDK-style libraries that temporarily targeted `net11.0;net10.0`, plus the sole test project `PythonVersionUnitTests`, which targeted `net10.0`. `TfmExport` and `TfmScanWithToken` already targeted only `net11.0`; `ProjectScannerDB.sqlproj` remains outside NuGet TFM management.
- All TFM properties are defined directly in project files. Dependency levels were recomputed and work was split leaf-to-root: tier 0: `Parameters`, `ConfidentialSettings`, `ProjectData`; tier 1: `RepoScan.DataModels`, `ConfigurationFileData`, `NuGetScanner`, `DefaultMetricsScanner`, `Storage`, `ReportsRefresh`, `ContentFilter`, `AzureDevOps`, `VisualStudioMetricsScanner`, `ProjectScannerSaveToSqlServer`, `FileFiltering`; tier 2: `RepoScan.Storage.SqlServer`, `Parser`, `FilterConfigurationFromConfigFile`; tier 3: `EndOfLifeConfigFile`, `YamlFileParser`, `PythonFileParser`, `VisualStudioFileParser`, `NpmFileParser`, `ProjectScanner`, `NuGetFileParser`, `OpenSecretsDetection`; tier 4: `AzureDevOpsScannerFramework`, `RepoScan.FileLocator`, `RuntimeMetricsScanner`, and `PythonVersionUnitTests`.
- Assessment summaries were consulted for all 29 scoped projects. Some include unrelated API/package findings; this consolidation did not change those APIs or package versions.
- Tier builds revealed NU1510 for explicit `Microsoft.Extensions.DependencyInjection.Abstractions` references in `ProjectData`, `RepoScan.Storage.SqlServer`, and `VisualStudioFileParser`. Those three references were removed because the package is provided automatically for `net11.0`; all three affected projects subsequently built with zero warnings and errors.
- No .NET-version-specific conditional compilation or TFM-conditioned items were found. The unrelated `#if DEBUG` in `TfmScanWithToken\Program.cs` was preserved.
- The 65 Python parser tests pass on `net11.0`. Central Package Management remains a post-migration recommendation: “All projects are now SDK-style and on a single TFM — CPM can be added cleanly without VersionOverride friction.” This applies to package-consuming .NET projects; the SQL database project remains outside NuGet TFM management.

## Execution Plan
Retarget every remaining library and test project to single `net11.0`, validate each dependency tier before proceeding, run the Python parser tests, and remove only package references proven redundant by restore diagnostics. Preserve non-TFM conditional behavior and defer CPM.

## Done when
All .NET application, library, and test projects target only `net11.0`; affected tests pass; no required non-TFM conditional behavior has been removed; the deferred CPM recommendation is documented.
