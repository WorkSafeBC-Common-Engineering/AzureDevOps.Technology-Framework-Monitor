# Progress: Consolidate tier-two libraries to .NET 11

## Changes
- Retargeted `RepoScan.Storage.SqlServer`, `Parser`, and `FilterConfigurationFromConfigFile` from `net11.0;net10.0` to single `net11.0`.
- Preserved package references and all other project metadata, including `FilterConfigurationFromConfigFile`'s `WCFMetadata` item.

## Validation
- Built all three projects after restore; all succeeded with no reported warnings or errors.
- Confirmed each now declares singular `<TargetFramework>net11.0</TargetFramework>` with no remaining `net10.0` target.
- No TFM-specific conditional source or project items were found or removed. `git diff --check` completed successfully; only task-documentation line-ending warnings were reported.
