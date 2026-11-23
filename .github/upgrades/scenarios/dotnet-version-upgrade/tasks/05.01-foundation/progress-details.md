# Progress: Consolidate foundation libraries to .NET 11

## Changes
- Retargeted `Parameters`, `ConfidentialSettings`, and `ProjectData` from `net11.0;net10.0` to a single `net11.0` target.
- Kept package references and all other project metadata unchanged. No TFM-specific conditional source or project items were found.

## Validation
- Built all three foundation projects with restore: all succeeded with no reported warnings or errors.
- Confirmed each project now declares singular `<TargetFramework>net11.0</TargetFramework>` and no longer references `net10.0`.
- `git diff --check` completed successfully. Git reported an existing line-ending warning for the unrelated `tasks/01-toolchain/task.md`; that file was not changed by this subtask.
