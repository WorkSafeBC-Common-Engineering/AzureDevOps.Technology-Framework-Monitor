CREATE VIEW [dbo].[ReportScan]
	AS
SELECT		O.[Name] AS [Organization],
			P.[ProjectId] AS [ProjectId],
			P.[Name] AS [ProjectName],
			R.[RepositoryId] AS [RepositoryId],
			R.[Name] AS RepositoryName,
			FT.[Id] AS [FileTypeId],
			F.[Id] AS [FileId],
			F.[Path] AS [FilePath],
			FPP.AzureFunction AS AzureFunctionRuntime,
			CASE WHEN FPP.AzureFunction LIKE 'v%' THEN 1 ELSE 0 END AS IsAzureFunction,
			EOL.[Display] AS [FrameworkVersion],
			CASE WHEN EOL.EOL IS NOT NULL AND EOL.EOL <= GETDATE() THEN 1 ELSE 0 END AS [IsEol],
			EOL.EOL AS [EolDate],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 3, GETDATE()) THEN 1 ELSE 0 END AS [Eol3Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 6, GETDATE()) THEN 1 ELSE 0 END AS [Eol6Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 9, GETDATE()) THEN 1 ELSE 0 END AS [Eol9Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 12, GETDATE()) THEN 1 ELSE 0 END AS [Eol12Months],
			CASE WHEN EOL IS NULL THEN 1
                 WHEN DATEDIFF(day, EOL, GETDATE()) > 0 THEN 0 ELSE 1 END AS [SupportedFramework],
			EOL.IsTargetVersion AS [TargetFramework],
			FR.[Name] AS [FileReference],
			FU.[Name] AS [UrlReference],
			FN.[PackageType] AS [PackageType],
			FN.[Name] AS [PackageReference],
			FN.Version AS [PackageVersion]

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
WHERE		P.NoScan = 0
	AND		R.NoScan = 0
	AND		P.Deleted = 0
	AND		R.Deleted = 0
