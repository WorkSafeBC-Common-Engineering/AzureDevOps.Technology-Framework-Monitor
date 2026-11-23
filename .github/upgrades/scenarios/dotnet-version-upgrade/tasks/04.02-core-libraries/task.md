# 04.02-core-libraries: Add .NET 11 targets to core parsing and storage libraries

## Objective
Multi-target core services that build on the foundation libraries.

## Scope
- `Parser\Parser.csproj`
- `Storage\Storage.csproj`
- `CodeFileFiltering\FileFiltering.csproj`
- `AzureDevOps\AzureDevOps.csproj`
- `DataModels\RepoScan.DataModels.csproj`
- `FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj`
- `ContentFilter\ContentFilter.csproj`
- `ProjectScannerFramework\ProjectScanner.csproj`

## Research Findings
- All eight projects define `TargetFramework` directly in SDK-style project files and currently target `net10.0`; dependency analysis found no imported local MSBuild targets or central package management.
- `Parser` references `ConfigurationFileData`, `Parameters`, and `ProjectData`; `Storage` references `ProjectData` and `ConfidentialSettings`; `FileFiltering`, `AzureDevOps`, `RepoScan.DataModels`, `FilterConfigurationFromConfigFile`, and `ContentFilter` reference the previously multi-targeted foundation libraries; `ProjectScanner` references `Parser`, `ProjectData`, and `ConfidentialSettings`.
- Assessment source-incompatibility counts are Parser 12, ProjectScanner 10, Storage 8, RepoScan.DataModels 4, and zero for the other four projects. The findings largely concern legacy `System.Configuration`; existing packages are at 11.0.0, which is retained unless restore/build evidence requires a change.
- `Parser`, `Storage`, `RepoScan.DataModels`, and `ProjectScanner` have active Unity service-locator/factory code. `ProjectData` also has active Unity factories, and `TfmScanWithToken\App.config` contains the Unity XML registrations. The selected built-in-DI migration crosses those scopes and requires a separately validated migration task; this batch focuses on retaining/building both TFMs.
- Existing package changes were preserved. Build both target frameworks for every project before completing this batch; no code or package change will be made solely because an analyzer reports an API finding.

## Steps
1. Review project assessments and source findings, especially Parser, ProjectScanner, Storage, and RepoScan.DataModels.
2. Add `net11.0` while retaining `net10.0`.
3. Restore and build both targets; fix compiler-confirmed API breaks and check flagged configuration behavior.

## Done when
All eight projects build for both TFMs and their project references resolve.
