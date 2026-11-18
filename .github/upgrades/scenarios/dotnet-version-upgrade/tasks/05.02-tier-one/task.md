# 05.02-tier-one: Consolidate tier-one libraries to .NET 11

## Objective
Remove the temporary .NET 10 target from all tier-one libraries after foundation tier validation.

## Scope
- `IT.TFM\DataModels\RepoScan.DataModels.csproj`
- `IT.TFM\ConfigurationFileData\ConfigurationFileData.csproj`
- `IT.TFM\NuGetScanner\NuGetScanner.csproj`
- `IT.TFM\DefaultMetricsScanner\DefaultMetricsScanner.csproj`
- `IT.TFM\Storage\Storage.csproj`
- `IT.TFM\ReportsRefresh\ReportsRefresh.csproj`
- `IT.TFM\ContentFilter\ContentFilter.csproj`
- `IT.TFM\AzureDevOps\AzureDevOps.csproj`
- `IT.TFM\VisualStudioMetricsScanner\VisualStudioMetricsScanner.csproj`
- `IT.TFM\ProjectScannerSaveToSqlServer\ProjectScannerSaveToSqlServer.csproj`
- `IT.TFM\CodeFileFiltering\FileFiltering.csproj`

## Steps
1. Change each multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`.
2. Restore and build the tier-one projects after tier 0 has passed.
3. Resolve errors or warnings caused by these project changes; leave unrelated package/API recommendations untouched.
4. Confirm no target-specific code path or project item needs removal.

## Done when
All 11 tier-one projects target only `net11.0` and build successfully before tier 2 is started.
