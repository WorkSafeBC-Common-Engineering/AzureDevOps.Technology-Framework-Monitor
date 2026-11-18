# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the solution's .NET application, library, and test projects from `net10.0` to `net11.0`.
**Scope**: 32 solution entries: 31 .NET projects and one SQL database project. `ProjectScannerDB.sqlproj` is an SSDT database definition and has no .NET runtime TFM; its SQL project metadata remains unchanged.

### Selected Strategy
**Top-Down (Application-First)** — Applications are upgraded first, shared libraries are multi-targeted temporarily, then consolidated.
**Rationale**: The 32-project solution has a deep dependency graph, and the assessment identifies source-incompatible and behavioral-change APIs across projects. Incremental application-first work supports validation at each boundary.

**Applications and tests**: `TfmExport` has no project dependencies; `ReportsRefresh` depends on `ConfidentialSettings` and is also referenced by `TfmScanWithToken`, so it temporarily supports both `net10.0` and `net11.0`; `TfmScanWithToken` depends on 19 projects. `PythonVersionUnitTests` tests `PythonFileParser` and `YamlFileParser` and will move with those libraries.

**Libraries**: The remaining 27 .NET libraries are upgraded in dependency order. Multi-targeting (`net10.0;net11.0`) is retained only during the transition so both old- and new-target consumers can build.

**Phase 2 trigger**: Begin library consolidation after all application projects target .NET 11 and every shared library has a validated `net11.0` target. Remove `net10.0` from libraries and from `ReportsRefresh`, then upgrade the Python parser test project before final validation.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | Top-Down (Application-First) | 32 projects and a deep dependency graph favor incremental application-first validation. |
| Package Management | Per-Project (defer CPM to post-migration) | The solution contains a legacy SQL project; keep package versions in individual projects during the TFM transition. |
| Unsupported Packages | Resolve Inline | The assessment flags one incompatible package, a small enough count to research and resolve inline. |
| Unsupported API Handling | Fix Inline | The assessment identifies 90 source-incompatible API findings; resolve changes in the upgrade rather than leaving stubs. |
| Dependency Injection | Migrate to Built-in DI Container | Unity is referenced across the solution; the selected approach aligns DI with .NET's built-in container. |

## Tasks

### 01-toolchain: Verify .NET 11 tooling and solution prerequisites

Confirm the .NET 11 SDK, repository SDK configuration, and solution build prerequisites before changing targets. Review the assessment and dependency graph, identify the SQL database project as a non-runtime exception, and preserve the pre-existing working-tree edits while integrating the package changes already underway.

**Done when**: The required .NET 11 SDK/build tooling is available or a specific blocker is documented; the target projects and SQL project exception are confirmed; existing edits are accounted for without being discarded.

### 02-lightweight-apps: Upgrade TfmExport and ReportsRefresh

Retarget `TfmExport` to `net11.0`. `TfmExport` has no project dependencies. `ReportsRefresh` references `ConfidentialSettings` and is also consumed by `TfmScanWithToken`, which remains on `net10.0` until the next application task; therefore, keep `ReportsRefresh` multi-targeted as `net11.0;net10.0` during the transition. The assessment reports no API compatibility incidents for these applications, but flags package-version recommendations; verify actual .NET 11 package compatibility rather than applying a suggested downgrade without evidence.

**Done when**: `TfmExport` targets `net11.0`; `ReportsRefresh` builds for both `net11.0` and `net10.0`; the .NET 10 consumer resolves its project reference; and relevant package changes are validated.

### 03-token-scanner-app: Upgrade TfmScanWithToken

Retarget `TfmScanWithToken` to `net11.0` after its smaller application dependencies have been checked. It references 19 projects and the assessment identifies six source-incompatible API findings for this application, plus package recommendations. Resolve the API and package issues inline, preserve the pre-existing project-file edits, and validate the application against its current library targets.

**Done when**: `TfmScanWithToken` targets `net11.0`, all 19 project references resolve, and the application builds with its reported API and package issues addressed.

### 04-shared-libraries: Add validated .NET 11 targets to shared libraries

Upgrade the 27 shared libraries in dependency order while retaining `net10.0` compatibility for consumers not yet migrated. The dependency graph ranges from foundation libraries (`Parameters`, `ProjectData`, and `ConfidentialSettings`) through parsing, storage, and scanner integration libraries. The solution assessment reports 90 source-incompatible API findings and 36 behavioral-change findings overall; address source changes inline, check behavior-sensitive `Uri`, XML serialization, configuration, and HTTP content use, and verify package compatibility against the actual target frameworks. Migrate Unity container usage to built-in DI where the projects actively use it.

**Done when**: Every shared library has a compiling `net11.0` target and retains a working `net10.0` target during transition; project references resolve at both TFMs; no source-incompatible API findings remain for the upgraded targets.

### 05-library-consolidation: Consolidate libraries and upgrade Python parser tests

After all application projects and shared library `net11.0` targets are validated, remove the temporary `net10.0` targets from libraries and from `ReportsRefresh`. Retarget `PythonVersionUnitTests` with `PythonFileParser` and `YamlFileParser`, run the affected tests, and remove only TFM-specific conditional code that is demonstrably obsolete. Preserve other conditional-compilation symbols and their existing behavior. Record Central Package Management as a post-migration recommendation: “All projects are now SDK-style and on a single TFM — CPM can be added cleanly without VersionOverride friction.” This recommendation applies to the .NET package-consuming projects; the SQL database project remains outside NuGet TFM management.

**Done when**: All .NET application, library, and test projects target only `net11.0`; affected tests pass; no required non-TFM conditional behavior has been removed; the deferred CPM recommendation is documented.

### 06-full-validation: Build and test the complete solution

Build the full solution and run its discovered tests after all target changes. Review final project targets and package references, confirm pre-existing changes were preserved or deliberately integrated, and ensure the unchanged SQL database project still uses its existing SSDT build configuration. Document any environment limitation or remaining incompatibility explicitly.

**Done when**: All .NET projects build for .NET 11 without warnings or errors, all runnable tests pass, the SQL project remains valid under its existing tooling, and no unresolved dependency incompatibilities remain.
