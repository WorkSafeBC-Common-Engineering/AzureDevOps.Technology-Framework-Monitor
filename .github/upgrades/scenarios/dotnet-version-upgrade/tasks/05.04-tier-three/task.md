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

## Research Findings
- Confirmed all eight project files declare `<TargetFrameworks>net11.0;net10.0</TargetFrameworks>` directly. The repository has no central `Directory.Build.props` TFM override.
- These projects depend on tier-0, tier-1, and tier-2 projects, all of which now build with only `net11.0`. `PythonFileParser` and `YamlFileParser` are also the production projects used by the final Python test tier.
- A scan of all eight source directories found no .NET-version-specific conditional compilation. No TFM-conditioned items were found; package references and other project metadata remain unchanged.
- Existing assessment API/package findings are outside this TFM-only change. There are no package updates or API edits planned.

## Steps
1. Change each multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`.
2. Restore and build the eight projects after tiers 0 through 2 have passed.
3. Resolve errors or warnings caused by these project changes; leave unrelated package/API recommendations untouched.
4. Confirm no target-specific code path or project item needs removal.

## Done when
All eight tier-three projects target only `net11.0` and build successfully before tier 4 is started.
