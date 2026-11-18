# 06-full-validation: Build and test the complete solution

Build the full solution and run its discovered tests after all target changes. Review final project targets and package references, confirm pre-existing changes were preserved or deliberately integrated, and ensure the unchanged SQL database project still uses its existing SSDT build configuration. Document any environment limitation or remaining incompatibility explicitly.

**Done when**: All .NET projects build for .NET 11 without warnings or errors, all runnable tests pass, the SQL project remains valid under its existing tooling, and no unresolved dependency incompatibilities remain.
