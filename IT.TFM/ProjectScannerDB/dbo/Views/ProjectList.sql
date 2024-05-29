CREATE VIEW [dbo].[ProjectList]
AS
SELECT	ProjectId,
		[Name]
FROM	dbo.Projects
WHERE	NoScan = 0
	AND	Deleted = 0