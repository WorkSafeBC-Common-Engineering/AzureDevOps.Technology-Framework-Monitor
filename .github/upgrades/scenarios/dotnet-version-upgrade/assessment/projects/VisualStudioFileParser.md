# VisualStudioFileParser\VisualStudioFileParser.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 3
- **Dependants**: 1
- **Number of Files**: 7
- **Number of Files with Incidents**: 2
- **Lines of Code**: 628
- **Estimated LOC to modify**: 1+ (at least 0.2% of the project)

## Related Projects

**Depends on (3)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ConfigurationFileData\ConfigurationFileData.csproj](../projects/ConfigurationFileData.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\Parser\Parser.csproj](../projects/Parser.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ProjectDataFramework\ProjectData.csproj](../projects/ProjectData.md)

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
    subgraph current["VisualStudioFileParser.csproj"]
        MAIN["<b>📦&nbsp;VisualStudioFileParser.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/VisualStudioFileParser.md"
    end
    subgraph downstream["Dependencies (3)"]
        P4["<b>📦&nbsp;Parser.csproj</b><br/><small>net10.0</small>"]
        P13["<b>📦&nbsp;ConfigurationFileData.csproj</b><br/><small>net10.0</small>"]
        P1["<b>📦&nbsp;ProjectData.csproj</b><br/><small>net10.0</small>"]
        click P4 "../projects/Parser.md"
        click P13 "../projects/ConfigurationFileData.md"
        click P1 "../projects/ProjectData.md"
    end
    P18 --> MAIN
    MAIN --> P4
    MAIN --> P13
    MAIN --> P1

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 1 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 502 |  |
| ***Total APIs Analyzed*** | ***503*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| System.Configuration.ConfigurationManager | 11.0.0 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

