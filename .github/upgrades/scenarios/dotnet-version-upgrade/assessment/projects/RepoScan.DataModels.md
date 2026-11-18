# DataModels\RepoScan.DataModels.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 4
- **Number of Files**: 19
- **Number of Files with Incidents**: 3
- **Lines of Code**: 577
- **Estimated LOC to modify**: 20+ (at least 3.5% of the project)

## Related Projects

**Depends on (1)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectDataFramework\ProjectData.csproj](../projects/ProjectData.md)

**Depended on by (4)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\FileLocator\RepoScan.FileLocator.csproj](../projects/RepoScan.FileLocator.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj](../projects/RepoScan.Storage.SqlServer.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\RuntimeMetricsScanner\RuntimeMetricsScanner.csproj](../projects/RuntimeMetricsScanner.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj](../projects/TfmScanWithToken.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (4)"]
        P15["<b>📦&nbsp;RepoScan.FileLocator.csproj</b><br/><small>net10.0</small>"]
        P17["<b>📦&nbsp;RepoScan.Storage.SqlServer.csproj</b><br/><small>net10.0</small>"]
        P18["<b>📦&nbsp;TfmScanWithToken.csproj</b><br/><small>net10.0</small>"]
        P30["<b>📦&nbsp;RuntimeMetricsScanner.csproj</b><br/><small>net10.0</small>"]
        click P15 "../projects/RepoScan.FileLocator.md"
        click P17 "../projects/RepoScan.Storage.SqlServer.md"
        click P18 "../projects/TfmScanWithToken.md"
        click P30 "../projects/RuntimeMetricsScanner.md"
    end
    subgraph current["RepoScan.DataModels.csproj"]
        MAIN["<b>📦&nbsp;RepoScan.DataModels.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/RepoScan.DataModels.md"
    end
    subgraph downstream["Dependencies (1)"]
        P1["<b>📦&nbsp;ProjectData.csproj</b><br/><small>net10.0</small>"]
        click P1 "../projects/ProjectData.md"
    end
    P15 --> MAIN
    P17 --> MAIN
    P18 --> MAIN
    P30 --> MAIN
    MAIN --> P1

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 4 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 16 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 524 |  |
| ***Total APIs Analyzed*** | ***544*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| System.Configuration.ConfigurationManager | 11.0.0 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

## Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Legacy Configuration System | 4 | 20.0% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

