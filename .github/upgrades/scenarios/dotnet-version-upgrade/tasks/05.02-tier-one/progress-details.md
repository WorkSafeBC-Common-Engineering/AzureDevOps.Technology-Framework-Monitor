# Progress: Consolidate tier-one libraries to .NET 11

## Changes
Retargeted all 11 tier-one projects from `net11.0;net10.0` to single `net11.0`:
- `DataModels`, `ConfigurationFileData`, `NuGetScanner`, `DefaultMetricsScanner`, and `Storage`
- `ReportsRefresh`, `ContentFilter`, `AzureDevOps`, `VisualStudioMetricsScanner`, `ProjectScannerSaveToSqlServer`, and `FileFiltering`

No package references, source behavior, or project metadata were changed. Preserved `ReportsRefresh` as an executable and retained the `VisualStudioMetricsScanner` `CopyMetricsExe` target.

## Validation
- Built each of the 11 projects after restore; all succeeded with no reported warnings or errors.
- Confirmed all 11 now declare singular `<TargetFramework>net11.0</TargetFramework>` and none retain `net10.0`.
- No TFM-specific conditional code or items were found or removed. `git diff --check` completed successfully; its only output was line-ending warnings for task documentation, including a pre-existing unrelated `tasks/01-toolchain/task.md` change.
