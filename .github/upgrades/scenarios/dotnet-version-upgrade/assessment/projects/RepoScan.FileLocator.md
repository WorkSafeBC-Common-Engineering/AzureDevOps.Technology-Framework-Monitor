# FileLocator\RepoScan.FileLocator.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 7
- **Dependants**: 1
- **Number of Files**: 7
- **Number of Files with Incidents**: 1
- **Lines of Code**: 825
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depends on (7)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\CodeFileFiltering\FileFiltering.csproj](../projects/FileFiltering.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\DataModels\RepoScan.DataModels.csproj](../projects/RepoScan.DataModels.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj](../projects/FilterConfigurationFromConfigFile.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ParameterSettings\Parameters.csproj](../projects/Parameters.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectDataFramework\ProjectData.csproj](../projects/ProjectData.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectScannerFramework\ProjectScanner.csproj](../projects/ProjectScanner.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\Storage\Storage.csproj](../projects/Storage.md)

**Depended on by (1)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj](../projects/TfmScanWithToken.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P18["<b>📦&nbsp;TfmScanWithToken.csproj</b><br/><small>net10.0</small>"]
        click P18 "../projects/TfmScanWithToken.md"
    end
    subgraph current["RepoScan.FileLocator.csproj"]
        MAIN["<b>📦&nbsp;RepoScan.FileLocator.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/RepoScan.FileLocator.md"
    end
    subgraph downstream["Dependencies (7)"]
        P14["<b>📦&nbsp;FileFiltering.csproj</b><br/><small>net10.0</small>"]
        P9["<b>📦&nbsp;Storage.csproj</b><br/><small>net10.0</small>"]
        P16["<b>📦&nbsp;RepoScan.DataModels.csproj</b><br/><small>net10.0</small>"]
        P12["<b>📦&nbsp;FilterConfigurationFromConfigFile.csproj</b><br/><small>net10.0</small>"]
        P25["<b>📦&nbsp;Parameters.csproj</b><br/><small>net10.0</small>"]
        P2["<b>📦&nbsp;ProjectScanner.csproj</b><br/><small>net10.0</small>"]
        P1["<b>📦&nbsp;ProjectData.csproj</b><br/><small>net10.0</small>"]
        click P14 "../projects/FileFiltering.md"
        click P9 "../projects/Storage.md"
        click P16 "../projects/RepoScan.DataModels.md"
        click P12 "../projects/FilterConfigurationFromConfigFile.md"
        click P25 "../projects/Parameters.md"
        click P2 "../projects/ProjectScanner.md"
        click P1 "../projects/ProjectData.md"
    end
    P18 --> MAIN
    MAIN --> P14
    MAIN --> P9
    MAIN --> P16
    MAIN --> P12
    MAIN --> P25
    MAIN --> P2
    MAIN --> P1

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 677 |  |
| ***Total APIs Analyzed*** | ***677*** |  |

