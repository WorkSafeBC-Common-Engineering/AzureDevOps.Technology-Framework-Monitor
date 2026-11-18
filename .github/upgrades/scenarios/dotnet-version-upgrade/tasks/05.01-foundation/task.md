# 05.01-foundation: Consolidate foundation libraries to .NET 11

## Objective
Remove the temporary .NET 10 target from the foundation libraries after confirming their current target properties.

## Scope
- `IT.TFM\ParameterSettings\Parameters.csproj`
- `IT.TFM\ConfidentialSettings\ConfidentialSettings.csproj`
- `IT.TFM\ProjectDataFramework\ProjectData.csproj`

## Research Findings
- Confirmed all three files define `<TargetFrameworks>net11.0;net10.0</TargetFrameworks>` directly in the project file. `get_project_dependencies` found no imported local MSBuild targets for these projects.
- `ProjectData.csproj` retains explicit references to `Microsoft.Extensions.DependencyInjection.Abstractions` and `System.Configuration.ConfigurationManager`; these are unrelated to TFM consolidation and remain unchanged. The other two projects have no explicit package references.
- A scan of all three source directories found no .NET-version-specific conditional compilation. The foundation projects have no TFM-conditioned items to remove.
- These are the dependency graph's tier-0 projects and have no project dependencies. Tier 1 remains blocked on their builds passing.

## Steps
1. Change each multi-targeting declaration from `net11.0;net10.0` to singular `<TargetFramework>net11.0</TargetFramework>`.
2. Restore and build the three projects. Resolve any errors or warnings caused by these project changes; do not make unrelated package or API updates.
3. Confirm no target-specific code path or project item needs removal.

## Done when
All three foundation projects target only `net11.0` and build successfully before tier 1 is started.
