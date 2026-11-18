# FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 2
- **Number of Files**: 2
- **Number of Files with Incidents**: 1
- **Lines of Code**: 52
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depends on (2)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ConfigurationFileData\ConfigurationFileData.csproj](../projects/ConfigurationFileData.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectDataFramework\ProjectData.csproj](../projects/ProjectData.md)

**Depended on by (2)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\FileLocator\RepoScan.FileLocator.csproj](../projects/RepoScan.FileLocator.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\TfmScanWithToken\TfmScanWithToken.csproj](../projects/TfmScanWithToken.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P15["<b>📦&nbsp;RepoScan.FileLocator.csproj</b><br/><small>net10.0</small>"]
        P18["<b>📦&nbsp;TfmScanWithToken.csproj</b><br/><small>net10.0</small>"]
        click P15 "../projects/RepoScan.FileLocator.md"
        click P18 "../projects/TfmScanWithToken.md"
    end
    subgraph current["FilterConfigurationFromConfigFile.csproj"]
        MAIN["<b>📦&nbsp;FilterConfigurationFromConfigFile.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/FilterConfigurationFromConfigFile.md"
    end
    subgraph downstream["Dependencies (2)"]
        P13["<b>📦&nbsp;ConfigurationFileData.csproj</b><br/><small>net10.0</small>"]
        P1["<b>📦&nbsp;ProjectData.csproj</b><br/><small>net10.0</small>"]
        click P13 "../projects/ConfigurationFileData.md"
        click P1 "../projects/ProjectData.md"
    end
    P15 --> MAIN
    P18 --> MAIN
    MAIN --> P13
    MAIN --> P1

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 14 |  |
| ***Total APIs Analyzed*** | ***14*** |  |

