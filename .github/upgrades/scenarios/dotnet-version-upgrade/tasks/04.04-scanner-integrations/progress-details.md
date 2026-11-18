# 04.04-scanner-integrations Progress

- Added `net11.0` while retaining `net10.0` in all nine scanner, storage, and persistence projects.
- Restores exposed vulnerable transitive `System.Security.Cryptography.Pkcs` versions in NuGetScanner (8.0.1 via NuGet.Packaging) and ProjectScannerSaveToSqlServer (9.0.4 via Microsoft.Data.SqlClient). Added patched same-major references 8.0.2 and 9.0.21; subsequent restores no longer emitted NU1902/NU1903.
- Fixed the nullable-annotation warning on `Pipeline.BlueprintType` using a scoped annotation context; the project builds without enabling nullable analysis across the legacy file.
- Validation: all nine projects built for both .NET 10 and .NET 11 with zero errors. After the warning fixes, ProjectScannerSaveToSqlServer built both targets with zero warnings/errors.
- Validation: the .NET 11 `TfmScanWithToken` application and its full project-reference graph built with zero warnings/errors.
- An IDE-triggered solution build encountered stale net10 restore assets for the not-yet-consolidated PythonVersionUnitTests project; the targeted SDK builds and test run previously completed successfully after restoring the test project. The final validation task will restore and build the complete solution after the test project moves to .NET 11.
