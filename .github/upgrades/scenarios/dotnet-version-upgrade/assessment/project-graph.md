# Projects Relationship Graph

[← Back to the assessment index](../assessment.md)

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;ProjectData.csproj</b><br/><small>net10.0</small>"]
    P2["<b>📦&nbsp;ProjectScanner.csproj</b><br/><small>net10.0</small>"]
    P3["<b>📦&nbsp;AzureDevOpsScannerFramework.csproj</b><br/><small>net10.0</small>"]
    P4["<b>📦&nbsp;Parser.csproj</b><br/><small>net10.0</small>"]
    P5["<b>📦&nbsp;VisualStudioFileParser.csproj</b><br/><small>net10.0</small>"]
    P6["<b>📦&nbsp;NuGetFileParser.csproj</b><br/><small>net10.0</small>"]
    P7["<b>⚙️&nbsp;ProjectScannerDB.sqlproj</b><br/><small>net472</small>"]
    P8["<b>📦&nbsp;ProjectScannerSaveToSqlServer.csproj</b><br/><small>net10.0</small>"]
    P9["<b>📦&nbsp;Storage.csproj</b><br/><small>net10.0</small>"]
    P10["<b>📦&nbsp;EndOfLifeConfigFile.csproj</b><br/><small>net10.0</small>"]
    P11["<b>📦&nbsp;ContentFilter.csproj</b><br/><small>net10.0</small>"]
    P12["<b>📦&nbsp;FilterConfigurationFromConfigFile.csproj</b><br/><small>net10.0</small>"]
    P13["<b>📦&nbsp;ConfigurationFileData.csproj</b><br/><small>net10.0</small>"]
    P14["<b>📦&nbsp;FileFiltering.csproj</b><br/><small>net10.0</small>"]
    P15["<b>📦&nbsp;RepoScan.FileLocator.csproj</b><br/><small>net10.0</small>"]
    P16["<b>📦&nbsp;RepoScan.DataModels.csproj</b><br/><small>net10.0</small>"]
    P17["<b>📦&nbsp;RepoScan.Storage.SqlServer.csproj</b><br/><small>net10.0</small>"]
    P18["<b>📦&nbsp;TfmScanWithToken.csproj</b><br/><small>net10.0</small>"]
    P19["<b>📦&nbsp;OpenSecretsDetection.csproj</b><br/><small>net10.0</small>"]
    P20["<b>📦&nbsp;NpmFileParser.csproj</b><br/><small>net10.0</small>"]
    P21["<b>📦&nbsp;AzureDevOps.csproj</b><br/><small>net10.0</small>"]
    P22["<b>📦&nbsp;ConfidentialSettings.csproj</b><br/><small>net10.0</small>"]
    P23["<b>📦&nbsp;ReportsRefresh.csproj</b><br/><small>net10.0</small>"]
    P24["<b>📦&nbsp;YamlFileParser.csproj</b><br/><small>net10.0</small>"]
    P25["<b>📦&nbsp;Parameters.csproj</b><br/><small>net10.0</small>"]
    P26["<b>📦&nbsp;NuGetScanner.csproj</b><br/><small>net10.0</small>"]
    P27["<b>📦&nbsp;TfmExport.csproj</b><br/><small>net10.0</small>"]
    P28["<b>📦&nbsp;DefaultMetricsScanner.csproj</b><br/><small>net10.0</small>"]
    P29["<b>📦&nbsp;VisualStudioMetricsScanner.csproj</b><br/><small>net10.0</small>"]
    P30["<b>📦&nbsp;RuntimeMetricsScanner.csproj</b><br/><small>net10.0</small>"]
    P31["<b>📦&nbsp;PythonFileParser.csproj</b><br/><small>net10.0</small>"]
    P32["<b>📦&nbsp;PythonVersionUnitTests.csproj</b><br/><small>net10.0</small>"]
    P2 --> P4
    P2 --> P1
    P2 --> P22
    P3 --> P21
    P3 --> P14
    P3 --> P4
    P3 --> P2
    P3 --> P1
    P4 --> P13
    P4 --> P25
    P4 --> P1
    P5 --> P4
    P5 --> P13
    P5 --> P1
    P6 --> P4
    P6 --> P1
    P8 --> P25
    P8 --> P1
    P9 --> P1
    P9 --> P22
    P10 --> P4
    P10 --> P13
    P10 --> P1
    P11 --> P1
    P12 --> P13
    P12 --> P1
    P13 --> P25
    P14 --> P1
    P15 --> P14
    P15 --> P9
    P15 --> P16
    P15 --> P12
    P15 --> P25
    P15 --> P2
    P15 --> P1
    P16 --> P1
    P17 --> P9
    P17 --> P16
    P17 --> P1
    P18 --> P20
    P18 --> P26
    P18 --> P5
    P18 --> P16
    P18 --> P29
    P18 --> P19
    P18 --> P17
    P18 --> P31
    P18 --> P15
    P18 --> P6
    P18 --> P8
    P18 --> P24
    P18 --> P30
    P18 --> P12
    P18 --> P28
    P18 --> P25
    P18 --> P11
    P18 --> P3
    P18 --> P23
    P19 --> P4
    P19 --> P1
    P20 --> P4
    P20 --> P1
    P21 --> P25
    P21 --> P22
    P23 --> P22
    P24 --> P4
    P26 --> P1
    P26 --> P22
    P28 --> P1
    P29 --> P1
    P30 --> P9
    P30 --> P16
    P30 --> P2
    P30 --> P1
    P30 --> P22
    P31 --> P4
    P31 --> P9
    P31 --> P1
    P32 --> P24
    P32 --> P31
    click P1 "projects/ProjectData.md"
    click P2 "projects/ProjectScanner.md"
    click P3 "projects/AzureDevOpsScannerFramework.md"
    click P4 "projects/Parser.md"
    click P5 "projects/VisualStudioFileParser.md"
    click P6 "projects/NuGetFileParser.md"
    click P7 "projects/ProjectScannerDB.md"
    click P8 "projects/ProjectScannerSaveToSqlServer.md"
    click P9 "projects/Storage.md"
    click P10 "projects/EndOfLifeConfigFile.md"
    click P11 "projects/ContentFilter.md"
    click P12 "projects/FilterConfigurationFromConfigFile.md"
    click P13 "projects/ConfigurationFileData.md"
    click P14 "projects/FileFiltering.md"
    click P15 "projects/RepoScan.FileLocator.md"
    click P16 "projects/RepoScan.DataModels.md"
    click P17 "projects/RepoScan.Storage.SqlServer.md"
    click P18 "projects/TfmScanWithToken.md"
    click P19 "projects/OpenSecretsDetection.md"
    click P20 "projects/NpmFileParser.md"
    click P21 "projects/AzureDevOps.md"
    click P22 "projects/ConfidentialSettings.md"
    click P23 "projects/ReportsRefresh.md"
    click P24 "projects/YamlFileParser.md"
    click P25 "projects/Parameters.md"
    click P26 "projects/NuGetScanner.md"
    click P27 "projects/TfmExport.md"
    click P28 "projects/DefaultMetricsScanner.md"
    click P29 "projects/VisualStudioMetricsScanner.md"
    click P30 "projects/RuntimeMetricsScanner.md"
    click P31 "projects/PythonFileParser.md"
    click P32 "projects/PythonVersionUnitTests.md"

```

