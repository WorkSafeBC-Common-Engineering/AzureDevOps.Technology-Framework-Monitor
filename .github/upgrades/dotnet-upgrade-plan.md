# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade ParameterSettings\Parameters.csproj
4. Upgrade ProjectDataFramework\ProjectData.csproj
5. Upgrade ConfigurationFileData\ConfigurationFileData.csproj
6. Upgrade ConfidentialSettings\ConfidentialSettings.csproj
7. Upgrade Parser\Parser.csproj
8. Upgrade ProjectScannerFramework\ProjectScanner.csproj
9. Upgrade CodeFileFiltering\FileFiltering.csproj
10. Upgrade AzureDevOps\AzureDevOps.csproj
11. Upgrade DataModels\RepoScan.DataModels.csproj
12. Upgrade Storage\Storage.csproj
13. Upgrade FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj
14. Upgrade AzureDevOpsScannerFramework\AzureDevOpsScannerFramework.csproj
15. Upgrade ContentFilter\ContentFilter.csproj
16. Upgrade DefaultMetricsScanner\DefaultMetricsScanner.csproj
17. Upgrade RuntimeMetricsScanner\RuntimeMetricsScanner.csproj
18. Upgrade YamlFileParser\YamlFileParser.csproj
19. Upgrade ProjectScannerSaveToSqlServer\ProjectScannerSaveToSqlServer.csproj
20. Upgrade NuGetFileParser\NuGetFileParser.csproj
21. Upgrade FileLocator\RepoScan.FileLocator.csproj
22. Upgrade RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj
23. Upgrade OpenSecretsDetection\OpenSecretsDetection.csproj
24. Upgrade VisualStudioMetricsScanner\VisualStudioMetricsScanner.csproj
25. Upgrade VisualStudioFileParser\VisualStudioFileParser.csproj
26. Upgrade NuGetScanner\NuGetScanner.csproj
27. Upgrade NpmFileParser\NpmFileParser.csproj
28. Upgrade TfmExport\TfmExport.csproj
29. Upgrade ReportsRefresh\ReportsRefresh.csproj
30. Upgrade TfmScanWithToken\TfmScanWithToken.csproj
31. Upgrade EndOfLifeConfigFile\EndOfLifeConfigFile.csproj

## Settings

### Excluded projects

| Project name | Description |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

| Package Name | Current Version | New Version | Description |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.EntityFrameworkCore | 8.0.16 | 10.0.0 | Recommended for .NET 10.0 |
| Microsoft.EntityFrameworkCore.Proxies | 8.0.16 | 10.0.0 | Recommended for .NET 10.0 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.16 | 10.0.0 | Recommended for .NET 10.0 |
| Microsoft.Extensions.Caching.Memory | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| Newtonsoft.Json | 13.0.3 | 13.0.4 | Recommended for .NET 10.0 |
| System.Collections.Immutable | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Configuration.ConfigurationManager | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Formats.Asn1 | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Reflection.Metadata | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Runtime.Caching | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Security.Permissions | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Text.Json | 9.0.5 | 10.0.0 | Recommended for .NET 10.0 |
| System.Threading.Tasks.Extensions | 4.6.3 | | Package functionality included with new framework reference |

### Project upgrade details

#### ParameterSettings\Parameters.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### ProjectDataFramework\ProjectData.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### ConfigurationFileData\ConfigurationFileData.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### ConfidentialSettings\ConfidentialSettings.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### Parser\Parser.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Security.Permissions should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### ProjectScannerFramework\ProjectScanner.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Security.Permissions should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### CodeFileFiltering\FileFiltering.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### AzureDevOps\AzureDevOps.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (recommended for .NET 10.0)
  - System.Text.Json should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### DataModels\RepoScan.DataModels.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### Storage\Storage.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### AzureDevOpsScannerFramework\AzureDevOpsScannerFramework.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Formats.Asn1 should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (recommended for .NET 10.0)
  - System.Collections.Immutable should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Reflection.Metadata should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Runtime.Caching should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### ContentFilter\ContentFilter.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### DefaultMetricsScanner\DefaultMetricsScanner.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### RuntimeMetricsScanner\RuntimeMetricsScanner.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### YamlFileParser\YamlFileParser.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### ProjectScannerSaveToSqlServer\ProjectScannerSaveToSqlServer.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.16` to `10.0.0` (recommended for .NET 10.0)
  - Microsoft.EntityFrameworkCore.Proxies should be updated from `8.0.16` to `10.0.0` (recommended for .NET 10.0)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `8.0.16` to `10.0.0` (recommended for .NET 10.0)
  - Microsoft.Extensions.Caching.Memory should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Formats.Asn1 should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### NuGetFileParser\NuGetFileParser.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### FileLocator\RepoScan.FileLocator.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### OpenSecretsDetection\OpenSecretsDetection.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### VisualStudioMetricsScanner\VisualStudioMetricsScanner.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### VisualStudioFileParser\VisualStudioFileParser.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### NuGetScanner\NuGetScanner.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Formats.Asn1 should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### NpmFileParser\NpmFileParser.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (recommended for .NET 10.0)
  - System.Threading.Tasks.Extensions should be removed (functionality included with new framework reference)

#### TfmExport\TfmExport.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### ReportsRefresh\ReportsRefresh.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### TfmScanWithToken\TfmScanWithToken.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.Caching.Memory should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)
  - System.Configuration.ConfigurationManager should be updated from `9.0.5` to `10.0.0` (recommended for .NET 10.0)

#### EndOfLifeConfigFile\EndOfLifeConfigFile.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
