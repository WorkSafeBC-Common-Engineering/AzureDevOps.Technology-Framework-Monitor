# 01-toolchain: Verify .NET 11 tooling and solution prerequisites

Confirm the .NET 11 SDK, repository SDK configuration, and solution build prerequisites before changing targets. Review the assessment and dependency graph, identify the SQL database project as a non-runtime exception, and preserve the pre-existing working-tree edits while integrating the package changes already underway.

## Research Findings
- The solution contains 32 projects: 31 .NET SDK-style projects currently targeting `net10.0` and `ProjectScannerDB.sqlproj`, an SSDT database definition with `OutputType=Database` and no runtime TFM.
- `ProjectScannerDB.sqlproj` imports Visual Studio SSDT targets and must remain on its SQL-specific project format rather than being retargeted to `net11.0`.
- The .NET SDK validation reports a compatible SDK and Visual Studio MSBuild at `C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe`; no `global.json` is present.
- The pre-existing worktree contains changes to 13 project files, including package-version updates. These remain in scope and must not be discarded.

**Done when**: The required .NET 11 SDK/build tooling is available or a specific blocker is documented; the target projects and SQL project exception are confirmed; existing edits are accounted for without being discarded.
