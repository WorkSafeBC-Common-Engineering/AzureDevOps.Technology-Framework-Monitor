# .NET 10.0 Upgrade Report

## Project target framework modifications

| Project name                                   | Old Target Framework    | New Target Framework         | Commits                   |
|:-----------------------------------------------|:-----------------------:|:----------------------------:|---------------------------|
| ParameterSettings\Parameters.csproj            |   net8.0                | net10.0                      | 78dbc5f7                  |
| ProjectDataFramework\ProjectData.csproj        |   net8.0                | net10.0                      | 11fda12d, 0dd9984f, 578a400f |
| ConfigurationFileData\ConfigurationFileData.csproj |   net8.0            | net10.0                      | 620ccad1, 707c7b5c         |
| ConfidentialSettings\ConfidentialSettings.csproj |   net8.0              | net10.0                      | 5568f0c9                  |
| Parser\Parser.csproj                           |   net8.0                | net10.0                      | f25acd9b, 4aded45d, 52294b86 |
| ProjectScannerFramework\ProjectScanner.csproj  |   net8.0                | net10.0                      | 0329c63a, f5b7fc90, 8aee7175 |
| CodeFileFiltering\FileFiltering.csproj         |   net8.0                | net10.0                      | d4779c1c, afb5559d         |
| AzureDevOps\AzureDevOps.csproj                 |   net8.0                | net10.0                      | 6bc29eb2, df63eca3, 5ad93d68, 9b1a0a49 |
| DataModels\RepoScan.DataModels.csproj          |   net8.0                | net10.0                      | 6bc29eb2, df63eca3, 9b1a0a49, 5ad93d68 |
| Storage\Storage.csproj                         |   net8.0                | net10.0                      | df63eca3, 9b1a0a49, 5ad93d68 |
| FilterConfigurationFromConfigFile\FilterConfigurationFromConfigFile.csproj | net8.0 | net10.0 | 653b68c1, 5e3cb18b |
| AzureDevOpsScannerFramework\AzureDevOpsScannerFramework.csproj | net8.0 | net10.0 | 728efff1, aa6185b1, 1b914859 |
| ContentFilter\ContentFilter.csproj             |   net8.0                | net10.0                      | 2bce6d28, d2d53f65         |
| DefaultMetricsScanner\DefaultMetricsScanner.csproj | net8.0              | net10.0                      | 73f73a74                  |
| RuntimeMetricsScanner\RuntimeMetricsScanner.csproj | net8.0              | net10.0                      | b582e3cc                  |
| YamlFileParser\YamlFileParser.csproj           |   net8.0                | net10.0                      | 6c484bd8                  |
| ProjectScannerSaveToSqlServer\ProjectScannerSaveToSqlServer.csproj | net8.0 | net10.0 | bc19782e, 42ecfea0, 5d146a34 |
| NuGetFileParser\NuGetFileParser.csproj         |   net8.0                | net10.0                      | b7df8e6f, 0875734b         |
| FileLocator\RepoScan.FileLocator.csproj        |   net8.0                | net10.0                      | e9fd7175, 9eb8adcc         |
| RepoScan.Storage.SqlServer\RepoScan.Storage.SqlServer.csproj | net8.0 | net10.0 | 8c2323c8, d9fadc53         |
| OpenSecretsDetection\OpenSecretsDetection.csproj | net8.0                | net10.0                      | 0b265f4b, 989f5e00         |
| VisualStudioMetricsScanner\VisualStudioMetricsScanner.csproj | net8.0 | net10.0 | 2a410d4f                  |
| VisualStudioFileParser\VisualStudioFileParser.csproj | net8.0              | net10.0                      | 99f28be7, 4def4718         |
| NuGetScanner\NuGetScanner.csproj               |   net8.0                | net10.0                      | 5053a898, 009486b2, 41da7c1a |
| NpmFileParser\NpmFileParser.csproj             |   net8.0                | net10.0                      | fa5b2dcf, 0aea19a1         |
| TfmExport\TfmExport.csproj                     |   net8.0                | net10.0                      | e18b874a, 8aa079f3         |
| ReportsRefresh\ReportsRefresh.csproj           |   net8.0                | net10.0                      | ee6ee422, 669bcf31         |
| TfmScanWithToken\TfmScanWithToken.csproj       |   net8.0                | net10.0                      | 7fcd28e2, fd90f24f         |
| EndOfLifeConfigFile\EndOfLifeConfigFile.csproj |   net8.0                | net10.0                      | b64043a1                  |

## NuGet Packages

| Package Name                        | Old Version | New Version | Commit Id                                 |
|:------------------------------------|:-----------:|:-----------:|-------------------------------------------|
| System.Configuration.ConfigurationManager | 9.0.5 | 10.0.0 | 707c7b5c, 4aded45d, f5b7fc90, 4def4718, e18b874a, 669bcf31, fd90f24f |
| System.Threading.Tasks.Extensions   | 4.6.3      | removed     | afb5559d, 0dd9984f, 4aded45d, f5b7fc90, 653b68c1, b7df8e6f, 9eb8adcc, d9fadc53, 989f5e00, d2d53f65 |
| Newtonsoft.Json                     | 13.0.3     | 13.0.4      | 6bc29eb2, aa6185b1, 0aea19a1             |
| System.Text.Json                    | 9.0.5      | 10.0.0      | 6bc29eb2                                 |
| System.Security.Permissions         | 9.0.5      | 10.0.0      | 4aded45d, f5b7fc90                       |
| System.Runtime.CompilerServices.Unsafe | 6.1.2   | removed     | 578a400f, 52294b86, 8aee7175, 5ad93d68   |
| System.Formats.Asn1                 | 9.0.5     | 10.0.0      | aa6185b1, 009486b2, 42ecfea0             |
| System.Collections.Immutable        | 9.0.5     | 10.0.0      | aa6185b1                                 |
| System.Reflection.Metadata          | 9.0.5     | 10.0.0      | aa6185b1                                 |
| System.Runtime.Caching              | 9.0.5     | 10.0.0      | aa6185b1                                 |
| Microsoft.EntityFrameworkCore       | 8.0.16    | 10.0.0      | 42ecfea0                                 |
| Microsoft.EntityFrameworkCore.Proxies | 8.0.16  | 10.0.0      | 42ecfea0                                 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.16 | 10.0.0      | 42ecfea0                                 |
| Microsoft.Extensions.Caching.Memory | 9.0.5     | 10.0.0      | 42ecfea0, fd90f24f                       |

## All commits

| Commit ID   | Description |
|:------------|:------------------------------------------------------------|
| 372b6a1c    | Commit upgrade plan                                        |
| 78dbc5f7    | Update Parameters.csproj to target .NET 10.0               |
| 11fda12d    | Update ProjectData.csproj to target .NET 10.0              |
| 0dd9984f    | Update ProjectData.csproj package references               |
| 578a400f    | Remove System.Runtime.CompilerServices.Unsafe from ProjectData.csproj |
| 620ccad1    | Update target framework to net10.0 in ConfigurationFileData.csproj |
| 707c7b5c    | Update ConfigurationFileData.csproj to use newer package   |
| 5568f0c9    | Update target framework in ConfidentialSettings.csproj     |
| f25acd9b    | Update target framework to net10.0 in Parser.csproj       |
| 4aded45d    | Update package versions in Parser.csproj                   |
| 52294b86    | Remove System.Runtime.CompilerServices.Unsafe from Parser.csproj |
| 0329c63a    | Update ProjectScanner.csproj to target .NET 10.0           |
| f5b7fc90    | Update ProjectScanner.csproj package versions and references |
| 8aee7175    | Remove System.Runtime.CompilerServices.Unsafe from ProjectScanner.csproj |
| d4779c1c    | Update target framework to net10.0 in FileFiltering.csproj |
| afb5559d    | Remove System.Threading.Tasks.Extensions from FileFiltering.csproj |
| 6bc29eb2    | Update package versions in AzureDevOps.csproj              |
| df63eca3    | Update target framework to .NET 10.0 in two projects       |
| 5ad93d68    | Remove unused package references from project files         |
| 9b1a0a49    | Update target framework to net10.0 in all projects         |
| 653b68c1    | Remove System.Threading.Tasks.Extensions from .csproj      |
| 5e3cb18b    | Update FilterConfigurationFromConfigFile.csproj to net10.0 |
| 728efff1    | Update AzureDevOpsScannerFramework.csproj to net10.0       |
| aa6185b1    | Update package versions in AzureDevOpsScannerFramework.csproj |
| 1b914859    | Remove unused package references in AzureDevOpsScannerFramework.csproj |
| 2bce6d28    | Update ContentFilter.csproj to target .NET 10.0            |
| d2d53f65    | Remove System.Threading.Tasks.Extensions from ContentFilter.csproj |
| 73f73a74    | Update DefaultMetricsScanner.csproj to target net10.0      |
| b582e3cc    | Update RuntimeMetricsScanner.csproj to target .NET 10.0    |
| 6c484bd8    | Update YamlFileParser.csproj to target .NET 10.0           |
| 5d146a34    | Remove System.Formats.Asn1 from ProjectScannerSaveToSqlServer.csproj |
| bc19782e    | Update target framework to net10.0 in ProjectScannerSaveToSqlServer.csproj |
| 42ecfea0    | Update NuGet package versions in ProjectScannerSaveToSqlServer.csproj |
| b7df8e6f    | Remove System.Threading.Tasks.Extensions from NuGetFileParser.csproj |
| 0875734b    | Update NuGetFileParser.csproj to target .NET 10.0          |
| e9fd7175    | Update target framework to net10.0 in RepoScan.FileLocator.csproj |
| 9eb8adcc    | Remove System.Threading.Tasks.Extensions from RepoScan.FileLocator.csproj |
| 8c2323c8    | Update target framework to net10.0 in RepoScan.Storage.SqlServer.csproj |
| d9fadc53    | Remove System.Threading.Tasks.Extensions from csproj        |
| 0b265f4b    | Update target framework to net10.0 in OpenSecretsDetection.csproj |
| 989f5e00    | Remove System.Threading.Tasks.Extensions from csproj        |
| 2a410d4f    | Update VisualStudioMetricsScanner.csproj to net10.0        |
| 99f28be7    | Update VisualStudioFileParser.csproj to target .NET 10.0   |
| 4def4718    | Bump System.Configuration.ConfigurationManager to 10.0.0 in VisualStudioFileParser.csproj |
| 5053a898    | Update NuGetScanner.csproj to target .NET 10.0             |
| 009486b2    | Bump System.Formats.Asn1 to 10.0.0 in NuGetScanner.csproj  |
| 41da7c1a    | Remove System.Formats.Asn1 from NuGetScanner.csproj        |
| fa5b2dcf    | Update NpmFileParser.csproj to target .NET 10.0            |
| 0aea19a1    | Update Newtonsoft.Json to 13.0.4 in NpmFileParser.csproj   |
| e18b874a    | Bump ConfigurationManager version in TfmExport.csproj      |
| 8aa079f3    | Update TfmExport.csproj to target .NET 10.0                |
| ee6ee422    | Update target framework to net10.0 in ReportsRefresh.csproj |
| 669bcf31    | Update ReportsRefresh.csproj: bump ConfigManager to 10.0.0 |
| 7fcd28e2    | Update TfmScanWithToken.csproj to target .NET 10.0         |
| fd90f24f    | Update package versions in TfmScanWithToken.csproj         |
| b64043a1    | Update target framework to net10.0 in EndOfLifeConfigFile.csproj |

## Project feature upgrades

All projects were upgraded to .NET 10.0. NuGet packages were updated or removed for compatibility. Unnecessary package references were cleaned up. No additional feature upgrades were required beyond those listed in the plan.

## Next steps

- Review and test all upgraded projects to ensure full compatibility and functionality on .NET 10.0.
- Monitor for any runtime or integration issues.
- Merge the `upgrade-to-NET10` branch into your main branch after validation.
