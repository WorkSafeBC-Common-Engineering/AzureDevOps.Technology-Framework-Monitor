# Progress: Replace active Unity service location with built-in DI

## Changes
- Replaced all nine Unity-backed static factories with required service and keyed-service lookups through a shared, explicitly initialized `IServiceProvider`; kept their signatures, key selection, initialization calls, and transient lifetime behavior.
- Added application registrations for file parsers, filters, scanners, metrics scanners, storage services, and the end-of-life checker. Added assembly-local registration extensions for internal Visual Studio parser and SQL storage implementations.
- Initialized the provider before application work begins. Kept it alive for the process lifetime to match the previous static container lifetime and avoid disposing transient storage services twice.
- Removed direct Unity package references and Unity XML configuration. Preserved `App.config` connection strings/app settings and `Parser\app.config`'s `FileReferences-Ignore` section.
- Added the missing direct `EndOfLifeConfigFile` project reference discovered while building registrations. Did not register the unused `IReadFileDetails` mismatch (`FileDetails` implements only `IWriteFileDetails`); stale TFS and credentials aliases also remain unregistered because they have no implementation/call sites.

## Validation
- `dotnet build IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj`: passed for .NET 11 with 0 warnings and 0 errors.
- Full solution build using Visual Studio MSBuild: passed. It reported two existing CS8618 warnings in `IT.TFM\TfmExport\Exporter.cs`, outside this task's changed projects.
- `dotnet test IT.TFM\Tests\UnitTests\PythonVersionUnitTests\PythonVersionUnitTests.csproj`: 65 passed on its current `net10.0` target.
- Attempting to run that test project on `net11.0` is blocked because the project itself still targets only `net10.0` (`NETSDK1005`); retargeting is scheduled in task 05.
- A source/config/project scan found no remaining Unity references under `IT.TFM`.
- Scanner execution was not run because it starts external Azure DevOps and SQL Server workloads. No unit tests currently exercise the static factory APIs.
