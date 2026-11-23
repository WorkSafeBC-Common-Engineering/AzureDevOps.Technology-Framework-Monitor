# ConfidentialSettings\ConfidentialSettings.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 6
- **Number of Files**: 1
- **Number of Files with Incidents**: 1
- **Lines of Code**: 42
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depended on by (6)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\AzureDevOps\AzureDevOps.csproj](../projects/AzureDevOps.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\NuGetScanner\NuGetScanner.csproj](../projects/NuGetScanner.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectScannerFramework\ProjectScanner.csproj](../projects/ProjectScanner.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ReportsRefresh\ReportsRefresh.csproj](../projects/ReportsRefresh.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\RuntimeMetricsScanner\RuntimeMetricsScanner.csproj](../projects/RuntimeMetricsScanner.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\Storage\Storage.csproj](../projects/Storage.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (6)"]
        P2["<b>📦&nbsp;ProjectScanner.csproj</b><br/><small>net10.0</small>"]
        P9["<b>📦&nbsp;Storage.csproj</b><br/><small>net10.0</small>"]
        P21["<b>📦&nbsp;AzureDevOps.csproj</b><br/><small>net10.0</small>"]
        P23["<b>📦&nbsp;ReportsRefresh.csproj</b><br/><small>net10.0</small>"]
        P26["<b>📦&nbsp;NuGetScanner.csproj</b><br/><small>net10.0</small>"]
        P30["<b>📦&nbsp;RuntimeMetricsScanner.csproj</b><br/><small>net10.0</small>"]
        click P2 "../projects/ProjectScanner.md"
        click P9 "../projects/Storage.md"
        click P21 "../projects/AzureDevOps.md"
        click P23 "../projects/ReportsRefresh.md"
        click P26 "../projects/NuGetScanner.md"
        click P30 "../projects/RuntimeMetricsScanner.md"
    end
    subgraph current["ConfidentialSettings.csproj"]
        MAIN["<b>📦&nbsp;ConfidentialSettings.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/ConfidentialSettings.md"
    end
    P2 --> MAIN
    P9 --> MAIN
    P21 --> MAIN
    P23 --> MAIN
    P26 --> MAIN
    P30 --> MAIN

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 86 |  |
| ***Total APIs Analyzed*** | ***86*** |  |

