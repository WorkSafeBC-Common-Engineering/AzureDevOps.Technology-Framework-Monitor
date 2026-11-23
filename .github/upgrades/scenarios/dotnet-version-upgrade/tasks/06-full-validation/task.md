# 06-full-validation: Build and test the complete solution

Build the full solution and run its discovered tests after all target changes. Review final project targets and package references, confirm pre-existing changes were preserved or deliberately integrated, and ensure the unchanged SQL database project still uses its existing SSDT build configuration. Document any environment limitation or remaining incompatibility explicitly.

## Research Findings
- Confirmed `IT.TFM\ProjectScanner.sln` contains 31 .NET projects and the separate `ProjectScannerDB.sqlproj` database project.
- Scanned all 31 `.csproj` files: each directly declares only `TargetFramework=net11.0`; none declares plural `TargetFrameworks`.
- SDK validation reports a compatible .NET 11 SDK and Visual Studio MSBuild at `C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe`.
- Test Explorer discovers 65 tests in `PythonVersionUnitTests`; the preceding tier validation passed all 65 tests on `net11.0`. Final task execution will rerun the discovered suite.
- The SQL project has no worktree changes and remains in the solution; verify its existing SSDT build setup without changing its project format or target.
- A prior full-solution build noted nullable initialization warnings in `IT.TFM\TfmExport\Exporter.cs` for `file` and `db`; re-evaluate them in the final build and resolve any remaining warnings without suppressions.
- Preserve the existing unrelated edit in `tasks/01-toolchain/task.md`; it is not part of this final validation task.

**Done when**: All .NET projects build for .NET 11 without warnings or errors, all runnable tests pass, the SQL project remains valid under its existing tooling, and no unresolved dependency incompatibilities remain.
