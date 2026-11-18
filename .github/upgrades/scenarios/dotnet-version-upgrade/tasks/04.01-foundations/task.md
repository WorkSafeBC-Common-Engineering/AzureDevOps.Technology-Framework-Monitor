# 04.01-foundations: Add .NET 11 targets to foundation libraries

## Objective
Multi-target the foundational libraries while preserving .NET 10 consumers.

## Scope
- `ParameterSettings\Parameters.csproj`
- `ConfidentialSettings\ConfidentialSettings.csproj`
- `ProjectDataFramework\ProjectData.csproj`
- `ConfigurationFileData\ConfigurationFileData.csproj`

## Research Findings
- All four projects define `<TargetFramework>` directly in their SDK-style project files and currently target `net10.0`; there is no centralized framework property.
- `Parameters` and `ConfidentialSettings` have no project dependencies. `ProjectData` has Unity and `System.Configuration.ConfigurationManager` references; `ConfigurationFileData` depends on `Parameters` and uses `System.Configuration.ConfigurationManager` 11.0.0.
- The assessment reports no source-incompatible APIs for `Parameters` or `ConfidentialSettings`, eight in `ProjectData`, and 37 in `ConfigurationFileData`, largely related to legacy `System.Configuration`. Building both TFMs is the compatibility check; no speculative API replacements are warranted if the compiler accepts these existing APIs.
- The existing package edits were retained. All four projects now target `net11.0;net10.0`, ordered newest first, and build with zero warnings and zero errors for both targets.

## Steps
1. Review each project's assessment details and existing project/package edits.
2. Add `net11.0` as the newest target while retaining `net10.0`.
3. Restore and build both targets; fix verified compatibility errors and preserve behavior.

## Done when
All four projects build for both TFMs and their project references resolve.
