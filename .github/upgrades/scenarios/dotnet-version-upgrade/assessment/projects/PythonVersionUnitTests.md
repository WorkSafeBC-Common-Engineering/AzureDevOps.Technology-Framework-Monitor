# Tests\UnitTests\PythonVersionUnitTests\PythonVersionUnitTests.csproj

[← Back to the assessment index](../../assessment.md)

## Project Info

- **Current Target Framework:** net10.0
- **Proposed Target Framework:** net11.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 2
- **Dependants**: 0
- **Number of Files**: 9
- **Number of Files with Incidents**: 1
- **Lines of Code**: 1240
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

## Related Projects

**Depends on (2)** — projects this one references:

- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\PythonParser\PythonFileParser.csproj](../projects/PythonFileParser.md)
- [C:\repos\WorkSafeBC-Common-Engineering\AzureDevOps.Technology-Framework-Monitor\IT.TFM\YamlFileParser\YamlFileParser.csproj](../projects/YamlFileParser.md)

## Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["PythonVersionUnitTests.csproj"]
        MAIN["<b>📦&nbsp;PythonVersionUnitTests.csproj</b><br/><small>net10.0</small>"]
        click MAIN "../projects/PythonVersionUnitTests.md"
    end
    subgraph downstream["Dependencies (2)"]
        P24["<b>📦&nbsp;YamlFileParser.csproj</b><br/><small>net10.0</small>"]
        P31["<b>📦&nbsp;PythonFileParser.csproj</b><br/><small>net10.0</small>"]
        click P24 "../projects/YamlFileParser.md"
        click P31 "../projects/PythonFileParser.md"
    end
    MAIN --> P24
    MAIN --> P31

```

## API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1555 |  |
| ***Total APIs Analyzed*** | ***1555*** |  |

## NuGet Package Issues

| Package | Current Version | Suggested Version | Severity | Issue |
| :--- | :---: | :---: | :---: | :--- |
| xunit | 2.9.3 | — | 🔵 Optional | NuGet package is deprecated |

Every project affected by these packages, and the versions the repository settles on: [aggregate NuGet packages](../nuget/aggregate-packages.md).

