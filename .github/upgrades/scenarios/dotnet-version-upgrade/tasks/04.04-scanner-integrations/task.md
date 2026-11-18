# 04.04-scanner-integrations: Add .NET 11 targets to scanner and persistence integrations

## Objective
Multi-target the remaining downstream scanner, storage, and persistence libraries.

## Scope
- `AzureDevOpsScannerFramework\AzureDevOpsScannerFramework.csproj`
- `DefaultMetricsScanner\DefaultMetricsScanner.csproj`
- `RuntimeMetricsScanner\RuntimeMetricsScanner.csproj`
- `ProjectScannerSaveToSqlServer\ProjectScannerSaveToSqlServer.csproj`
- `FileLocator\RepoScan.FileLocator.csproj`
- `RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj`
- `OpenSecretsDetection\OpenSecretsDetection.csproj`
- `VisualStudioMetricsScanner\VisualStudioMetricsScanner.csproj`
- `NuGetScanner\NuGetScanner.csproj`

## Research Findings
- Confirmed all nine projects are SDK-style and define `net10.0` directly in their project files; dependency analysis found no imported local MSBuild targets overriding the TFM. Preserve the staged compatibility target (`net10.0`) while adding `net11.0`.
- Direct references: `AzureDevOpsScannerFramework` -> AzureDevOps, FileFiltering, Parser, ProjectScanner, ProjectData; `DefaultMetricsScanner` -> ProjectData; `RuntimeMetricsScanner` -> ConfidentialSettings, RepoScan.DataModels, ProjectScanner, ProjectData, Storage; `ProjectScannerSaveToSqlServer` -> Parameters, ProjectData; `RepoScan.FileLocator` -> FileFiltering, RepoScan.DataModels, FilterConfigurationFromConfigFile, Parameters, ProjectScanner, ProjectData, Storage; `RepoScan.Storage.SqlServer` -> RepoScan.DataModels, ProjectData, Storage; `OpenSecretsDetection` -> Parser, ProjectData; `VisualStudioMetricsScanner` -> ProjectData; `NuGetScanner` -> ProjectData, ConfidentialSettings. Their referenced libraries are already multi-targeted.
- Existing direct package references include `System.Configuration.ConfigurationManager` 11.0.0, `System.Runtime.Caching` 10.0.5, `Microsoft.Data.SqlClient` 7.1.2, and `Newtonsoft.Json` 13.0.4 in AzureDevOpsScannerFramework; EF Core packages 10.0.5 and `Microsoft.Extensions.Caching.Memory` 11.0.0 in ProjectScannerSaveToSqlServer; `Microsoft.CodeAnalysis.Metrics` 5.6.0 in VisualStudioMetricsScanner; and NuGet.Packaging/NuGet.Protocol 7.6.0 in NuGetScanner. The assessment's 10.0.15 recommendations are advisories, not demonstrated incompatibilities; other existing package versions were retained.
- Restore exposed vulnerable transitive Pkcs versions: NuGet.Packaging 7.6.0 resolved 8.0.1 and EFCore.SqlServer 10.0.5 -> Microsoft.Data.SqlClient 6.1.1 resolved 9.0.4. Added same-major patched direct references (8.0.2 and 9.0.21 respectively) to clear NU1902/NU1903.
- Assessment reports zero binary incompatibilities; source-compatibility flags are 2 for AzureDevOpsScannerFramework and 3 for OpenSecretsDetection, with no build-blocking location identified. Behavioral-change flags are 1 for ProjectScannerSaveToSqlServer and 8 for NuGetScanner; retain behavior and verify all TFMs compile. Other projects have no flagged API changes. `VisualStudioMetricsScanner` also has a custom post-build target copying Metrics tool files; preserve it while changing only the TFM.
- The first clean compile also reported CS8632 for the nullable `PipelineType?` navigation property in `Pipeline.cs`; scoped nullable-annotation directives preserve that property's nullable contract without enabling nullable analysis across the legacy file.

## Steps
1. Review per-project assessments, focusing on scanner APIs, behavior-sensitive changes, and package advisories.
2. Add `net11.0` while retaining `net10.0`.
3. Restore and build both targets and the dependent application; resolve verified issues without suppressing warnings.

## Done when
All nine projects build for both TFMs, project references resolve, and the .NET 11 application consumer builds.
