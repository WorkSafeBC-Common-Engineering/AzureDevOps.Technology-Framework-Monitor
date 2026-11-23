# Progress: 02-lightweight-apps

## Changes
- Retargeted `IT.TFM\TfmExport\TfmExport.csproj` to `net11.0`.
- Multi-targeted `IT.TFM\ReportsRefresh\ReportsRefresh.csproj` to `net11.0;net10.0` so the still-`net10.0` `TfmScanWithToken` project can consume it during the transition.
- Retained the existing `Microsoft.Data.SqlClient` 7.1.2 and `System.Configuration.ConfigurationManager` 11.0.0 package references; restore and builds validated these references for `net11.0`.

## Validation
- `ReportsRefresh` built successfully for both `net11.0` and `net10.0` after a forced restore regenerated its assets.
- `TfmScanWithToken` built successfully for `net10.0` and resolved the `ReportsRefresh` project reference.
- Visual Studio MSBuild built the full `ProjectScanner.sln` successfully, including the SSDT database project.
- Test Explorer returned no tests for `TfmExport` or `ReportsRefresh`.

## Issues and notes
- An initial restore left stale assets, causing `NETSDK1005`; `dotnet restore --force` regenerated both targets. The IDE build tool continued to report stale project-reference metadata, so validation used the installed Visual Studio MSBuild directly; that full-solution build passed.
- The SDK `dotnet build` solution command cannot load the SQL project because it does not include Visual Studio's SSDT targets. This is a tooling limitation, not a project failure; Visual Studio MSBuild completed the full solution build.
- The full build reported pre-existing package vulnerability warnings in `NuGetScanner` and `ProjectScannerSaveToSqlServer`. These projects were not modified by this task.
