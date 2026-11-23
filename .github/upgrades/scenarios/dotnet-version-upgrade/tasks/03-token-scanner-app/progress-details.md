# Progress: 03-token-scanner-app

## Changes
- Retargeted `IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj` from `net10.0` to `net11.0`.
- Kept the existing package references and legacy configuration behavior. `ReportsRefresh`'s temporary `net10.0` target continues to serve the old consumer and its `net11.0` target is selected by the upgraded application.

## Validation
- `dotnet build IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj` succeeded; output was produced under `bin\Debug\net11.0` and all 19 project references resolved.
- Visual Studio MSBuild built the full `ProjectScanner.sln` successfully, including the SSDT database project.
- No test project or tests are defined for `TfmScanWithToken`.

## Issues and notes
- The assessment's six source-incompatibility flags did not produce source/compiler errors when the application and its full dependency graph were built for .NET 11; no speculative source changes were made.
- The build reports pre-existing NU1902/NU1903 vulnerability advisories in `NuGetScanner` and `ProjectScannerSaveToSqlServer`, plus unrelated nullable warnings. They are outside this application's source changes and remain for their owning project tasks.
