# ConfigurationFileData\ConfigurationFileData.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 4
- **Number of Files**: 2
- **Number of Files with Incidents**: 2
- **Lines of Code**: 160
- **Estimated LOC to modify**: 37+ (at least 23.1% of the project)

## Related Projects

**Depends on (1)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\ParameterSettings\Parameters.csproj](../projects/Parameters.md)

**Depended on by (4)** — projects that reference this one:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\EndOfLifeConfigFile\EndOfLifeConfigFile.csproj](../projects/EndOfLifeConfigFile.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj](../projects/FilterConfigurationFromConfigFile.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\Parser\Parser.csproj](../projects/Parser.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\VisualStudioFileParser\VisualStudioFileParser.csproj](../projects/VisualStudioFileParser.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (4)"]
        P4["<b>📦&nbsp;Parser.csproj</b><br/><small>net10.0</small>"]
        P5["<b>📦&nbsp;VisualStudioFileParser.csproj</b><br/><small>net10.0</small>"]
        P10["<b>📦&nbsp;EndOfLifeConfigFile.csproj</b><br/><small>net10.0</small>"]
        P12["<b>📦&nbsp;FilterConfigurationFromConfigFile.csproj</b><br/><small>net10.0</small>"]
        click P4 "../projects/Parser.md"
        click P5 "../projects/VisualStudioFileParser.md"
        click P10 "../projects/EndOfLifeConfigFile.md"
        click P12 "../projects/FilterConfigurationFromConfigFile.md"
    end
    subgraph current["ConfigurationFileData.csproj"]
        MAIN["<b>📦&nbsp;ConfigurationFileData.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/ConfigurationFileData.md"
    end
    subgraph downstream["Dependencies (1)"]
        P25["<b>📦&nbsp;Parameters.csproj</b><br/><small>net10.0</small>"]
        click P25 "../projects/Parameters.md"
    end
    P4 --> MAIN
    P5 --> MAIN
    P10 --> MAIN
    P12 --> MAIN
    MAIN --> P25

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 37 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 118 |  |
| ***Total APIs Analyzed*** | ***155*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| System.Configuration.ConfigurationManager | 11.0.0 | 10.0.15 | 🟡 Potential | NuGet package upgrade is recommended |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

## Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Legacy Configuration System | 37 | 100.0% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

