CREATE VIEW [dbo].[FilePropertiesPivot]
AS
SELECT		Id AS FileId, AndroidApp, DBSchemaProvider, Error, ExeName32, FileFormat, iOSApp,
						  MajorVersion, MinimumVisualStudioVersion, MinorVersion, OutputType,
						  ProjectVersion, RevisionVersion, SchemaVersion, Sdk, TargetFramework,
						  TargetFrameworkVersion, TargetLanguage, ToolsVersion, [Type], VisualStudioVersion,
						  [ApiKey Open Secret], [DB Open Secret], AzureFunction
FROM		(	SELECT		F.Id, FP.Name, ISNULL(FP.Value, '') AS Value
				FROM		Files F
				INNER JOIN	FileProperties FP ON F.Id = FP.FileId AND FP.PropertyTypeId = 1
			) AS SourceTable
PIVOT
			(
				MAX([Value])
				FOR [Name] IN (	AndroidApp, DBSchemaProvider, Error, ExeName32, FileFormat, iOSApp,
								MajorVersion, MinimumVisualStudioVersion, MinorVersion, OutputType,
								ProjectVersion, RevisionVersion, SchemaVersion, Sdk, TargetFramework,
								TargetFrameworkVersion, TargetLanguage, ToolsVersion, [Type], VisualStudioVersion,
								[ApiKey Open Secret], [DB Open Secret], AzureFunction)
			) AS PivotTable
