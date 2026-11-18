# 05.05-tier-four-tests: Consolidate final libraries and retarget Python tests

## Objective
Remove the temporary .NET 10 targets from the final dependent libraries, retarget the Python parser tests to .NET 11, and run those tests.

## Scope
- `IT.TFM\AzureDevOpsScannerFramework\AzureDevOpsScannerFramework.csproj`
- `IT.TFM\FileLocator\RepoScan.FileLocator.csproj`
- `IT.TFM\RuntimeMetricsScanner\RuntimeMetricsScanner.csproj`
- `IT.TFM\Tests\UnitTests\PythonVersionUnitTests\PythonVersionUnitTests.csproj`
- Remove redundant .NET 11 `Microsoft.Extensions.DependencyInjection.Abstractions` references from `IT.TFM\ProjectDataFramework\ProjectData.csproj`, `IT.TFM\RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj`, and `IT.TFM\VisualStudioFileParser\VisualStudioFileParser.csproj` after final restore diagnostics.

## Research Findings
- Confirmed the three final libraries currently declare `<TargetFrameworks>net11.0;net10.0</TargetFrameworks>` and the Python test project declares `<TargetFramework>net10.0</TargetFramework>` directly. The test project has no imported local MSBuild targets or central TFM override.
- The three libraries depend on the previously validated tiers. The test project references `PythonFileParser` and `YamlFileParser`, both now single-targeted on `net11.0`.
- Test setup uses `Microsoft.NET.Test.Sdk` 17.14.1 and xUnit 2.9.3; assessment marks xUnit deprecated as an optional recommendation. Test discovery currently finds 65 tests on `net10.0`.
- Final .NET 11 test/build restore exposed NU1510 warnings for explicit `Microsoft.Extensions.DependencyInjection.Abstractions` references in `ProjectData`, `RepoScan.Storage.SqlServer`, and `VisualStudioFileParser`. Dependency inspection confirms these references are directly declared, and the .NET 11 SDK says the package is automatically available; the code uses the framework-provided DI extension APIs. Remove only these redundant references and verify all affected projects still compile.
- No .NET-version-specific conditional source or TFM-conditioned items were found in the four project directories. Preserve other conditional behavior, including the unrelated `#if DEBUG` in the application.
- Central Package Management remains deferred; the parent task records the requested post-migration recommendation.

## Steps
1. Change each library's multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`. Change the test project's `<TargetFramework>` from `net10.0` to `net11.0`.
2. Remove the three redundant DI abstractions package references identified above, preserving all other package references.
3. Restore and build the affected tier-four projects and the three projects whose package reference was removed; resolve warnings or errors caused by these changes.
4. Run `PythonVersionUnitTests` on `net11.0` and investigate failures.
5. Confirm no target-specific code path or project item needs removal; preserve `#if DEBUG` and any other non-TFM conditional behavior.
6. Keep Central Package Management deferred and retain its recommendation in the parent task documentation.

## Done when
All four tier-four projects target only `net11.0`, the Python parser tests pass, the three DI package-reference removals build without NU1510, and no non-TFM behavior was removed.
