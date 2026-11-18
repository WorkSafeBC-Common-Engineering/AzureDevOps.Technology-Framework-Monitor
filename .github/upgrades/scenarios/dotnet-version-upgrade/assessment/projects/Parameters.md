# ParameterSettings\Parameters.csproj

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
- **Lines of Code**: 7
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depended on by (6)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\AzureDevOps\AzureDevOps.csproj](../projects/AzureDevOps.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ConfigurationFileData\ConfigurationFileData.csproj](../projects/ConfigurationFileData.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\FileLocator\RepoScan.FileLocator.csproj](../projects/RepoScan.FileLocator.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\Parser\Parser.csproj](../projects/Parser.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectScannerSaveToSqlServer\ProjectScannerSaveToSqlServer.csproj](../projects/ProjectScannerSaveToSqlServer.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj](../projects/TfmScanWithToken.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (6)"]
        P4["<b>📦&nbsp;Parser.csproj</b><br/><small>net10.0</small>"]
        P8["<b>📦&nbsp;ProjectScannerSaveToSqlServer.csproj</b><br/><small>net10.0</small>"]
        P13["<b>📦&nbsp;ConfigurationFileData.csproj</b><br/><small>net10.0</small>"]
        P15["<b>📦&nbsp;RepoScan.FileLocator.csproj</b><br/><small>net10.0</small>"]
        P18["<b>📦&nbsp;TfmScanWithToken.csproj</b><br/><small>net10.0</small>"]
        P21["<b>📦&nbsp;AzureDevOps.csproj</b><br/><small>net10.0</small>"]
        click P4 "../projects/Parser.md"
        click P8 "../projects/ProjectScannerSaveToSqlServer.md"
        click P13 "../projects/ConfigurationFileData.md"
        click P15 "../projects/RepoScan.FileLocator.md"
        click P18 "../projects/TfmScanWithToken.md"
        click P21 "../projects/AzureDevOps.md"
    end
    subgraph current["Parameters.csproj"]
        MAIN["<b>📦&nbsp;Parameters.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/Parameters.md"
    end
    P4 --> MAIN
    P8 --> MAIN
    P13 --> MAIN
    P15 --> MAIN
    P18 --> MAIN
    P21 --> MAIN

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 4 |  |
| ***Total APIs Analyzed*** | ***4*** |  |

