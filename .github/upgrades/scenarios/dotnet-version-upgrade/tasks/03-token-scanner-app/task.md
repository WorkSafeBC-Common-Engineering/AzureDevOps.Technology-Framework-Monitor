# 03-token-scanner-app: Upgrade TfmScanWithToken

Retarget `TfmScanWithToken` to `net11.0` after its smaller application dependencies have been checked. It references 19 projects and the assessment identifies six source-incompatible API findings for this application, plus package recommendations. Resolve the API and package issues inline, preserve the pre-existing project-file edits, and validate the application against its current library targets.

## Research Findings
- `TfmScanWithToken.csproj` is SDK-style and declares its target directly. It currently targets `net10.0`, references 19 projects, and has three direct package references; the repository uses per-project package versions rather than central package management.
- The current direct package versions are `Microsoft.CodeAnalysis.Metrics` 5.6.0, `Microsoft.Extensions.Caching.Memory` 11.0.0, and `System.Configuration.ConfigurationManager` 11.0.0. The latter two have assessment recommendations to use 10.0.15; the existing 11.0.0 versions already restore successfully in the `net11.0` app upgraded in the previous task, so retain them pending direct compatibility evidence.
- `ReportsRefresh` now supports `net11.0;net10.0`; its `net10.0` target preserves the existing consumer path until this application is retargeted. The other referenced libraries currently target `net10.0`, which should remain consumable by this `net11.0` application while the libraries are migrated in the next task.
- The application source is `Program.cs`; it uses `System.Configuration.ConfigurationManager.AppSettings` for legacy XML settings, matching the assessment's legacy configuration finding. `App.config` also contains the Unity registrations and SQL/app settings; do not remove or rewrite those as part of the TFM change without a concrete build/runtime requirement.
- The build should be checked for all target-compatibility and source errors. If the reported compatibility items do not reproduce during build, preserve behavior rather than making speculative source edits.

**Done when**: `TfmScanWithToken` targets `net11.0`, all 19 project references resolve, and the application builds with its reported API and package issues addressed.
