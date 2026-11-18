# 05.04-tier-three: Consolidate tier-three libraries to .NET 11

## Objective
Remove the temporary .NET 10 target from tier-three libraries after tier-two validation.

## Scope
- `IT.TFM\EndOfLifeConfigFile\EndOfLifeConfigFile.csproj`
- `IT.TFM\YamlFileParser\YamlFileParser.csproj`
- `IT.TFM\PythonParser\PythonFileParser.csproj`
- `IT.TFM\VisualStudioFileParser\VisualStudioFileParser.csproj`
- `IT.TFM\NpmFileParser\NpmFileParser.csproj`
- `IT.TFM\ProjectScannerFramework\ProjectScanner.csproj`
- `IT.TFM\NuGetFileParser\NuGetFileParser.csproj`
- `IT.TFM\OpenSecretsDetection\OpenSecretsDetection.csproj`

## Steps
1. Change each multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`.
2. Restore and build the eight projects after tiers 0 through 2 have passed.
3. Resolve errors or warnings caused by these project changes; leave unrelated package/API recommendations untouched.
4. Confirm no target-specific code path or project item needs removal.

## Done when
All eight tier-three projects target only `net11.0` and build successfully before tier 4 is started.
