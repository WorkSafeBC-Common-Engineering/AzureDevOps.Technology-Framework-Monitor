CREATE VIEW [analysis].dimOrganizations AS
SELECT 
    o.Id AS OrganizationId,
    o.Name AS OrganizationName,
    o.Uri AS OrganizationUri
FROM dbo.Organizations o
