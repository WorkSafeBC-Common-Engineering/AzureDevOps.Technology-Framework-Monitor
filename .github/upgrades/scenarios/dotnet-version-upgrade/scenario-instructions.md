# Scenario Instructions

## Upgrade Parameters
- **Target framework**: `net11.0`
- **Scope**: Entire solution `IT.TFM\ProjectScanner.sln`
- **Flow mode**: Automatic

## Upgrade Options

### Strategy
- Upgrade Strategy: Top-Down (Application-First)

### Project Structure
- Package Management: Per-Project (defer CPM to post-migration)

### Compatibility
- Unsupported Packages: Resolve Inline (1 incompatible package flagged)
- Unsupported API Handling: Fix Inline

### Modernization
- Dependency Injection: Migrate to Built-in DI Container

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: The solution contains 32 projects and a deep dependency graph; application-first upgrades enable incremental validation.

### Execution Constraints
- Upgrade entry-point applications before consolidating shared libraries.
- Keep shared libraries on their current target until application upgrades are validated; multi-target only when a real consumer compatibility requirement is found. `ReportsRefresh` temporarily supports both `net10.0` and `net11.0` because `TfmScanWithToken` initially targets `net10.0`.
- Consolidate remaining library targets and remove the temporary `net10.0` target from `ReportsRefresh` after all applications are upgraded.
- Update and run tests alongside the projects they exercise, then validate the full solution.
- Preserve non-TFM conditional compilation and document unresolved purpose rather than removing it.

## Source Control
- **Source and working branch**: `feature/am56655/updatePackages` (stay on current branch)
- **Pre-existing working-tree changes**: Preserve and integrate as needed; do not discard.
- **Commit strategy**: After Each Task (top-down strategy recommendation; previously Manual).

## User Preferences
### Execution Style
- Upgrade every project in the solution to .NET 11; use judgment to keep or change existing edits as necessary.
