# 02-lightweight-apps: Upgrade TfmExport and ReportsRefresh

Retarget `TfmExport` to `net11.0`. `ReportsRefresh` references `ConfidentialSettings.csproj` and is also consumed by `TfmScanWithToken`, which remains on `net10.0` until the next application task; keep `ReportsRefresh` multi-targeted as `net11.0;net10.0` during this transition. The assessment reports no API compatibility incidents for these applications, but flags package-version recommendations; verify actual .NET 11 package compatibility rather than applying a suggested downgrade without evidence.

## Research Findings
- `TfmExport.csproj` and `ReportsRefresh.csproj` are SDK-style executable projects. Both declare their target framework directly in the project file and enable nullable reference types.
- `TfmExport` has no project references. `ReportsRefresh` references `ConfidentialSettings.csproj` and is referenced by `TfmScanWithToken.csproj`, so it must retain `net10.0` until that consumer is upgraded.
- Both projects use direct `PackageReference` entries for `Microsoft.Data.SqlClient` 7.1.2 and `System.Configuration.ConfigurationManager` 11.0.0. The dependency analysis found no central package management.
- Per-project assessment queries show zero API issues for both apps; each reports one `System.Configuration.ConfigurationManager` package recommendation to use 10.0.15. Because the projects are moving to `net11.0` and the existing 11.0.0 references are pre-existing user edits, retain them unless restore/build evidence shows they are incompatible.
- `TfmExport` has one mandatory TFM change and one package recommendation; `ReportsRefresh` has one mandatory TFM change and one package recommendation. Test Coverage is not recommended for either project.
- The full solution baseline build succeeded before this task; the first post-change build exposed `NU1201` because the .NET 10 consumer could not reference a `net11.0`-only `ReportsRefresh`. Multi-targeting `ReportsRefresh` as `net11.0;net10.0` resolved the compatibility issue after a forced restore regenerated its assets.
- Visual Studio MSBuild built the full solution successfully, including both `ReportsRefresh` targets, `TfmExport`, and the `TfmScanWithToken` consumer. The SDK `dotnet build` solution path cannot build the SSDT project because it does not include Visual Studio's SQL targets; the VS MSBuild build is authoritative. The only test filters for these two apps returned no tests.

**Done when**: `TfmExport` targets `net11.0`; `ReportsRefresh` builds for both `net11.0` and `net10.0`; the .NET 10 consumer resolves its project reference; and relevant package changes are validated.
