# 05.03-tier-two: Consolidate tier-two libraries to .NET 11

## Objective
Remove the temporary .NET 10 target from tier-two libraries after tier-one validation.

## Scope
- `IT.TFM\RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj`
- `IT.TFM\Parser\Parser.csproj`
- `IT.TFM\FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj`

## Steps
1. Change each multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`.
2. Restore and build the three projects after tiers 0 and 1 have passed.
3. Resolve errors or warnings caused by these project changes; leave unrelated package/API recommendations untouched.
4. Confirm no target-specific code path or project item needs removal.

## Done when
All three tier-two projects target only `net11.0` and build successfully before tier 3 is started.
