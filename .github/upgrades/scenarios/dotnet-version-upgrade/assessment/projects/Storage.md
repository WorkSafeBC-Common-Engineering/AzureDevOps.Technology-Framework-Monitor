# Storage\Storage.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 4
- **Number of Files**: 3
- **Number of Files with Incidents**: 3
- **Lines of Code**: 105
- **Estimated LOC to modify**: 8+ (at least 7.6% of the project)

## Related Projects

**Depends on (2)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ConfidentialSettings\ConfidentialSettings.csproj](../projects/ConfidentialSettings.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectDataFramework\ProjectData.csproj](../projects/ProjectData.md)

**Depended on by (4)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\FileLocator\RepoScan.FileLocator.csproj](../projects/RepoScan.FileLocator.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\PythonParser\PythonFileParser.csproj](../projects/PythonFileParser.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj](../projects/RepoScan.Storage.SqlServer.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\RuntimeMetricsScanner\RuntimeMetricsScanner.csproj](../projects/RuntimeMetricsScanner.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (4)"]
        P15["<b>📦&nbsp;RepoScan.FileLocator.csproj</b><br/><small>net10.0</small>"]
        P17["<b>📦&nbsp;RepoScan.Storage.SqlServer.csproj</b><br/><small>net10.0</small>"]
        P30["<b>📦&nbsp;RuntimeMetricsScanner.csproj</b><br/><small>net10.0</small>"]
        P31["<b>📦&nbsp;PythonFileParser.csproj</b><br/><small>net10.0</small>"]
        click P15 "../projects/RepoScan.FileLocator.md"
        click P17 "../projects/RepoScan.Storage.SqlServer.md"
        click P30 "../projects/RuntimeMetricsScanner.md"
        click P31 "../projects/PythonFileParser.md"
    end
    subgraph current["Storage.csproj"]
        MAIN["<b>📦&nbsp;Storage.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/Storage.md"
    end
    subgraph downstream["Dependencies (2)"]
        P1["<b>📦&nbsp;ProjectData.csproj</b><br/><small>net10.0</small>"]
        P22["<b>📦&nbsp;ConfidentialSettings.csproj</b><br/><small>net10.0</small>"]
        click P1 "../projects/ProjectData.md"
        click P22 "../projects/ConfidentialSettings.md"
    end
    P15 --> MAIN
    P17 --> MAIN
    P30 --> MAIN
    P31 --> MAIN
    MAIN --> P1
    MAIN --> P22

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 8 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 83 |  |
| ***Total APIs Analyzed*** | ***91*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| System.Configuration.ConfigurationManager | 11.0.0 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

## Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Legacy Configuration System | 8 | 100.0% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

