CREATE VIEW [dbo].[PipelineReport]
AS 

SELECT		P.Name AS Project,
			CASE WHEN R.Name IS NULL THEN ''
			     ELSE R.Name END AS Repository,
			PI.Name AS Pipeline,
			CASE WHEN PI.Type = 'yaml' THEN 'YAML'
			     ELSE 'Classic' END AS Type,
			CASE WHEN PI.Type = 'yaml' AND PI.YamlType IS NULL THEN 'Custom'
			     WHEN PI.Type <> 'yaml' AND PI.YamlType IS NULL THEN ''
			     ELSE PI.YamlType END AS [YAML Type],
			CASE PI.PipelineType 
				 WHEN 'release' THEN 'Release' 
				 WHEN 'build' THEN 'Build'
				 ELSE '' END AS [Classic Type],
			CASE WHEN PI.LastRunStart IS NULL THEN ''
				 ELSE CONVERT(nvarchar, PI.LastRunStart, 23) END AS [LastRun],
			CASE WHEN PI.State IS NULL THEN ''
			     ELSE PI.State END AS State,
			CASE WHEN PI.Result IS NULL THEN ''
			     ELSE PI.Result END AS Result,
			CASE WHEN PI.Portfolio IS NULL THEN ''
			     ELSE PI.Portfolio END AS Portfolio,
			CASE WHEN PI.Product IS NULL THEN ''
			     ELSE PI.Product END AS Product,
			PI.Folder,
			CASE WHEN PI.YamlType = 'V2' THEN 'Healthy'
				 ELSE 'Life-Support' END AS Status,
			CASE WHEN PI.Folder LIKE '%black-hole%' THEN 'Noise'
				 WHEN PI.LastRunStart IS NULL THEN 'Noise'
				 WHEN PI.LastRunStart < DATEADD(YEAR, -2, GETDATE()) THEN 'Noise'
				 WHEN PI.LastRunStart < DATEADD(YEAR, -1, GETDATE()) AND PI.LastRunStart >= DATEADD(YEAR, -2, GETDATE()) THEN 'Suspect'
			     ELSE 'In-Use' END AS Relevance

				 
			--,PI.*
FROM		Pipelines PI
INNER JOIN	Projects P ON PI.ProjectId = P.Id
LEFT JOIN	Repositories R ON PI.RepositoryId = R.Id

--ORDER BY	P.Name, R.Name, PI.Name