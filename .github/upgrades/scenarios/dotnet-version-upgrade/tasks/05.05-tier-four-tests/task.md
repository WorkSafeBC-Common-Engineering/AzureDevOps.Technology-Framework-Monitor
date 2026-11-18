# 05.05-tier-four-tests: Consolidate final libraries and retarget Python tests

## Objective
Remove the temporary .NET 10 targets from the final dependent libraries, retarget the Python parser tests to .NET 11, and run those tests.

## Scope
- `IT.TFM\AzureDevOpsScannerFramework\AzureDevOpsScannerFramework.csproj`
- `IT.TFM\FileLocator\RepoScan.FileLocator.csproj`
- `IT.TFM\RuntimeMetricsScanner\RuntimeMetricsScanner.csproj`
- `IT.TFM\Tests\UnitTests\PythonVersionUnitTests\PythonVersionUnitTests.csproj`

## Steps
1. Change each library's multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`. Change the test project's `<TargetFramework>` from `net10.0` to `net11.0`.
2. Restore and build the four projects after tiers 0 through 3 have passed.
3. Run `PythonVersionUnitTests` on `net11.0` and investigate failures.
4. Confirm no target-specific code path or project item needs removal; preserve `#if DEBUG` and any other non-TFM conditional behavior.
5. Keep Central Package Management deferred and retain its recommendation in the parent task documentation.

## Done when
All four tier-four projects target only `net11.0`, the Python parser tests pass, and no non-TFM behavior was removed.
