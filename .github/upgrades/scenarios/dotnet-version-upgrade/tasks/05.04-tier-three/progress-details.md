# Progress: Consolidate tier-three libraries to .NET 11

## Changes
Retargeted all eight tier-three projects from `net11.0;net10.0` to single `net11.0`:
- `EndOfLifeConfigFile`, `YamlFileParser`, `PythonFileParser`, and `VisualStudioFileParser`
- `NpmFileParser`, `ProjectScanner`, `NuGetFileParser`, and `OpenSecretsDetection`

No package references or source code were changed.

## Validation
- Built all eight projects after restore; all succeeded with no reported warnings or errors.
- Confirmed all eight now declare singular `<TargetFramework>net11.0</TargetFramework>` and none retain `net10.0`.
- No .NET-version-specific conditional source or TFM-conditioned items were found or removed. `git diff --check` completed successfully; only task-documentation line-ending warnings were reported.
