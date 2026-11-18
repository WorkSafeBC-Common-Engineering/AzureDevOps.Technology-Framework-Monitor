# Progress: Consolidate libraries and upgrade Python parser tests

## Changes
- Converted all 28 temporarily multi-targeted libraries to singular `net11.0` and retargeted `PythonVersionUnitTests` to `net11.0`.
- Removed redundant .NET 11 `Microsoft.Extensions.DependencyInjection.Abstractions` references from `ProjectData`, `RepoScan.Storage.SqlServer`, and `VisualStudioFileParser` after NU1510 restore diagnostics. Preserved other package references and project behavior.
- Confirmed no TFM-specific conditional code/items needed removal; retained the unrelated `#if DEBUG` behavior.
- Documented Central Package Management as deferred: “All projects are now SDK-style and on a single TFM — CPM can be added cleanly without VersionOverride friction.”

## Validation
- Built every dependency tier as it was retargeted; all targeted builds succeeded.
- Built the three projects with redundant DI references removed; each completed with 0 warnings and 0 errors.
- Ran the Python parser test suite on `net11.0`: 65 passed, 0 failed, 0 skipped.
- Confirmed no `.csproj` under `IT.TFM` retains `net10.0` or plural `TargetFrameworks`. `ProjectScannerDB.sqlproj` remains unchanged.
- `git diff --check` succeeded; Git only reported existing task-documentation line-ending warnings.
