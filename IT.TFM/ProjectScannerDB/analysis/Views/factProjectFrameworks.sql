CREATE VIEW [analysis].[factProjectFrameworks]
AS
SELECT		
			F.Id AS FileId,			
			EOL.[Display] AS [Package Version],
			CASE WHEN EOL.EOL IS NOT NULL AND EOL.EOL <= GETDATE() THEN 'YES' ELSE '' END AS [Package - EOL],
			EOL.EOL AS [Package - Date EOL],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 3, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 3 Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 6, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 6 Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 9, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 9 Months],
			CASE WHEN EOL.EOL <= DATEADD(MONTH, 12, GETDATE()) THEN 'YES' ELSE '' END AS [Package EOL - 12 Months],
			CASE WHEN EOL IS NULL THEN 1
                 WHEN DATEDIFF(day, EOL, GETDATE()) > 0 THEN 0 ELSE 1 END AS [Is Supported Version],
			EOL.IsTargetVersion AS [Is Target Version]

FROM		Files F
LEFT JOIN	FilePropertiesPivot FPP ON F.Id = FPP.FileId
LEFT JOIN	FileReferences FN ON F.Id = FN.FileId AND FN.FileReferenceTypeId = 3
LEFT JOIN	dotNetEndOfLife EOL ON (FPP.TargetFramework = EOL.[Version] OR FPP.TargetFrameworkVersion = EOL.[Version] 
	-- this is to help with combining 'angular/core [major version]' as we do not need minor version for EOL calculation
	OR FN.Name + ' ' + LEFT(Fn.Version, abs(charindex('.', Fn.Version) - 1))  = EOL.Version)
WHERE		EOL.Display IS NOT NULL
