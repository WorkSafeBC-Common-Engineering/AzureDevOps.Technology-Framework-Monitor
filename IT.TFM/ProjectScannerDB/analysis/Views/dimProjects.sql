CREATE VIEW [analysis].dimProjects AS
SELECT  p.Id AS ProjectId,
        p.Name AS ProjectName,
        p.Abbreviation,
        p.Description,
        p.LastUpdate,
        p.Revision,
        p.Url AS ProjectUrl,
        p.OrganizationId
FROM    dbo.Projects p
WHERE   Deleted = 0
    AND NoScan = 0;