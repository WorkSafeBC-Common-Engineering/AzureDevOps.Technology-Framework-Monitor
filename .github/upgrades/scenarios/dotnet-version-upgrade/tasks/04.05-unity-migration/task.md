# 04.05-unity-migration: Replace active Unity service location with built-in DI

## Objective
Migrate the selected Unity usage to Microsoft's built-in dependency-injection container without changing registrations, selection behavior, or lifetimes.

## Scope
- `ProjectDataFramework\FilterConfiguration.cs` and `Filters.cs`
- `DataModels\StorageFactory.cs`
- `Parser\ContentParserFactory.cs`, `FileParserFactory.cs`, and `EndOfLifeCheckerFactory.cs`
- `Storage\StorageFactory.cs`
- `ProjectScannerFramework\ScannerFactory.cs` and `MetricsScannerFactory.cs`
- `TfmScanWithToken\App.config` Unity registration section and related composition/startup code
- Unity package references in projects that no longer use Unity

## Research Findings
- `TfmScanWithToken` is the application composition root (net11.0) and references all active scanner/parser/storage implementations. Static factories read the Unity section from `App.config`; preserve app settings and connection strings while replacing only Unity composition.
- Active Unity resolution is in nine factory classes: `FilterConfiguration`, `Filters`, `DataModels.StorageFactory`, `ContentParserFactory`, `FileParserFactory`, `EndOfLifeCheckerFactory`, `Storage.StorageFactory`, `ScannerFactory`, and `MetricsScannerFactory`. Call sites across FileLocator, Parser, PythonParser, VisualStudioFileParser, RuntimeMetricsScanner, and RepoScan.Storage.SqlServer rely on their static signatures. No explicit lifetime managers or property injection were found; Unity defaults every registration to transient. Use keyed built-in registrations for named resolutions and retain the static factory APIs through a shared, explicitly initialized `IServiceProvider` accessor as a compatibility seam.
- `TfmScanWithToken\App.config` defines file-parser keys (including VBProject -> CSProject), scanner keys, metrics-scanner keys, filter/content-parser keys, storage reader/writer keys, and service types. `Parser\app.config` duplicates a smaller file-parser mapping and has an unrelated `FileReferences-Ignore` section; remove only Unity configuration and preserve the latter. The `TfsScanner` and credentials aliases have no implementation or code call sites in this solution and are not registered by active factory calls.
- Unity package references are direct in ProjectData, Parser, Storage, RepoScan.DataModels, and ProjectScanner; all have the same Unity 5.11.x versions. Remove those references after replacing their uses. The built-in provider accessor needs `Microsoft.Extensions.DependencyInjection.Abstractions`; TfmScanWithToken needs the DI container package. VisualStudioFileParser and RepoScan.Storage.SqlServer contain internal implementations, so keep them internal and expose assembly-local registration extensions.
- The Unity config maps `IReadFileDetails` to `RepoScan.Storage.SqlServer.FileDetails`, but that class implements only `IWriteFileDetails`; no call site uses `GetFileDetailsReader`. Do not create a false DI mapping; preserve the valid read/write registrations and record this pre-existing unused mismatch.
- No unit tests reference these factory APIs. Python parser tests exercise parser behavior separately. Runtime scan paths require external Azure DevOps credentials and SQL Server, so validation will build and resolve the registered services without making external connections.

## Scope Inventory
- Projects touched: `ProjectDataFramework`, `Parser`, `Storage`, `DataModels`, `ProjectScannerFramework`, `VisualStudioFileParser`, `RepoScan.Storage.SqlServer`, and the `TfmScanWithToken` application. The first five have direct Unity package references; the two implementation assemblies currently see Unity transitively. All library projects already target `net11.0;net10.0`; the application targets `net11.0`, so this task changes composition, not TFMs.
- Source/configuration surfaces confirmed: nine static factories (`FilterConfiguration`, `Filters`, `StorageFactory` in DataModels and Storage, `ContentParserFactory`, `FileParserFactory`, `EndOfLifeCheckerFactory`, `ScannerFactory`, and `MetricsScannerFactory`), `TfmScanWithToken\Program.cs`, `TfmScanWithToken\App.config`, and `Parser\app.config`. New composition surfaces will be the ProjectData provider accessor, assembly-local registration extensions for internal parser/storage types, and application-level registrations.
- Direct-reference gap discovered: the app config named `EndOfLifeConfigFile.Checker`, but `TfmScanWithToken` had no project reference to `EndOfLifeConfigFile`. Added the project reference so the built-in registration is compiled and the implementation is deployed with the application.
- Unity registrations use transient/default lifetimes. Preserve keyed registrations and the `VBProject` -> `CSProject` alias. Keep app settings/connection strings and `FileReferences-Ignore`; exclude the unused `IReadFileDetails` registration because `FileDetails` does not implement that interface. No service is registered with property injection or a non-transient lifetime.
- Assessment consultation: all eight projects are SDK-style and targeted for `net11.0`. Unity packages are assessed as compatible; project issue counts are ProjectData 13, Parser 15, Storage 10, DataModels 22, ProjectScanner 13, VisualStudioFileParser 3, RepoScan.Storage.SqlServer 1, and TfmScanWithToken 9. Remaining issues concern the separate framework/API/configuration and package-upgrade work, not the Unity-to-DI migration. Existing `System.Configuration.ConfigurationManager` references remain needed for non-Unity app settings.
- Validation must build the affected application dependency graph with restore after package edits, check that no Unity API/config references remain, and run the available Python parser tests. Do not execute scanner workloads that require Azure DevOps credentials or SQL Server.

## Steps
1. Inventory all Unity registration names, implementation types, constructor dependencies, and lifetimes in the config and factories.
2. Implement built-in DI composition and adapt named service resolution while preserving current behavior.
3. Remove obsolete Unity packages/configuration only after all callers build and registration parity is verified.
4. Build the full app dependency graph and run available tests; explicitly document any runtime-only checks that require service credentials or a database.

## Done when
No active Unity container or Unity configuration usage remains in application/library source, all former registrations resolve through built-in DI, and the .NET 11 application builds with behavior-preserving service selection.
