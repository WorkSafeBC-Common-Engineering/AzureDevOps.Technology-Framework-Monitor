# Progress: Consolidate final libraries and retarget Python tests

## Changes
- Retargeted `AzureDevOpsScannerFramework`, `RepoScan.FileLocator`, `RuntimeMetricsScanner`, and `PythonVersionUnitTests` to single `net11.0`.
- Removed redundant `Microsoft.Extensions.DependencyInjection.Abstractions` references from `ProjectData`, `RepoScan.Storage.SqlServer`, and `VisualStudioFileParser` after restore reported NU1510 for those references under .NET 11. Other package references were preserved.
- No source code or non-TFM conditional behavior was changed. The deferred Central Package Management recommendation remains documented in the parent task.

## Validation
- Built the four tier-four projects and all three projects with the removed DI package references; all succeeded.
- Direct `dotnet build` of `ProjectData`, `RepoScan.Storage.SqlServer`, and `VisualStudioFileParser` completed with 0 warnings and 0 errors each.
- `dotnet test IT.TFM\Tests\UnitTests\PythonVersionUnitTests\PythonVersionUnitTests.csproj` ran on `net11.0`: 65 passed, 0 failed, 0 skipped. The direct CLI run had no restore warnings.
- Visual Studio Test Explorer also reported 65/65 passed but logged transient AnyCode test-container refresh/semaphore errors; the direct CLI run independently confirmed the tests pass.
- A repository-wide project-file scan found no remaining `net10.0` target or plural `TargetFrameworks` declarations in `.csproj` files. The SQL database project is intentionally excluded.
- `git diff --check` completed successfully; only line-ending warnings for task documentation were reported.
