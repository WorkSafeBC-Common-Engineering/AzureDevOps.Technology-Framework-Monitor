CREATE VIEW [dbo].[FullScan]
AS
SELECT		'AzureDevOps' AS ScanSource,
			O.[Name] AS [Org. Name],
			P.[Name] AS [Project Name], P.ProjectId AS [Project ID], P.[Description] AS [Project Description],
			P.LastUpdate AS [Project Last Update], P.Revision AS [Project Revision], P.[State] AS [Project State],
			P.[Url] AS [Project Url], P.Visibility AS [Project Visibility], P.Deleted AS [Project Deleted],
			R.[Name] AS [Repository Name], R.[Portfolio] AS [Portfolio Name], R.[ApplicationProjectName] AS [Application Project Name],			
			R.[ComponentName] AS [Component Name],R.RepositoryId AS [Repository ID],
			R.DefaultBranch AS [Repository Default Branch], R.[Url] AS [Repository URL], R.Deleted AS [Repository Deleted],
			FT.[Value] AS [File Type], F.FileId AS [File ID], F.[Path] AS [File Path], F.[Url] AS [File URL],

			FPP.AndroidApp AS PROP_AndroidApp, FPP.DBSchemaProvider AS PROP_DBSchemaProvider, FPP.Error AS PROP_Error,
			FPP.ExeName32 AS PROP_ExeName32, FPP.FileFormat AS PROP_FileFormat, FPP.iOSApp AS PROP_iOSApp,
			FPP.MajorVersion AS PROP_MajorVersion, FPP.MinimumVisualStudioVersion AS PROP_MinimumVisualStudioVersion,
			FPP.MinorVersion AS PROP_MinorVersion, FPP.OutputType AS PROP_OutputType, 
			FPP.ProjectVersion AS PROP_ProjectVersion, FPP.RevisionVersion AS PROP_RevisionVersion,
			FPP.SchemaVersion AS PROP_SchemaVersion, FPP.Sdk AS PROP_Sdk, FPP.TargetFramework AS PROP_TargetFramework,
			FPP.TargetFrameworkVersion AS PROP_TargetFrameworkVersion, FPP.TargetLanguage AS PROP_TargetLanguage,
			FPP.ToolsVersion AS PROP_ToolsVersion, FPP.[Type] AS PROP_Type, FPP.VisualStudioVersion AS PROP_VisualStudioVersion,
			FPP.AzureFunction AS PROP_AzureFunction,
			CASE WHEN FPP.AzureFunction LIKE 'v%' THEN 'Y' ELSE '' END AS IsAzureFunction,
			FPP.[ApiKey Open Secret], FPP.[DB Open Secret],
			
			EOL.[Display] AS [Package Version],
			CASE WHEN EOL.EOL IS NOT NULL AND EOL.EOL <= GETDATE() THEN 'YES' ELSE '' END AS [Package - EOL],
			EOL.EOL AS [Package - Date EOL],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 3, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 3 Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 6, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 6 Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 9, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 9 Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 12, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 12 Months],

			FR.[Name] AS [File Reference],
			FU.[Name] AS [URL Reference],
			FN.[PackageType] AS [Pkg Type],
			FN.[Name] AS [Pkg Reference],
			FN.Version AS [Pkg Version],
			FN.VersionComparator as [Version Comparator],
			FN.FrameworkVersion AS [Pkg Framework Version]

FROM		Organizations O
INNER JOIN	Projects P ON O.Id = P.OrganizationId
INNER JOIN	Repositories R ON P.Id = R.ProjectId
INNER JOIN	Files F ON R.Id = F.RepositoryId
INNER JOIN	FileTypes FT ON F.FileTypeId = FT.Id
LEFT JOIN	FilePropertiesPivot FPP ON F.Id = FPP.FileId
LEFT JOIN	FileReferences FR ON F.Id = FR.FileId AND FR.FileReferenceTypeId = 1
LEFT JOIN	FileReferences FU ON F.Id = FU.FileId AND FU.FileReferenceTypeId = 2
LEFT JOIN	FileReferences FN ON F.Id = FN.FileId AND FN.FileReferenceTypeId = 3
LEFT JOIN	dotNetEndOfLife EOL ON (FPP.TargetFramework = EOL.[Version] OR FPP.TargetFrameworkVersion = EOL.[Version] 
	-- this is to help with combining 'angular/core [major version]' as we do not need minor version for EOL calculation
	OR FN.Name + ' ' + LEFT(Fn.Version, abs(charindex('.', Fn.Version) - 1))  = EOL.Version)




	