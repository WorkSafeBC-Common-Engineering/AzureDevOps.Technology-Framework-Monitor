CREATE VIEW [analysis].dimRepositories AS
SELECT  r.Id AS RepositoryId,
        r.Name AS RepositoryName,
        r.DefaultBranch,
        r.Url,
        r.Size,
        r.LastCommitId,
        r.ProjectId
FROM    dbo.Repositories r
WHERE   Deleted = 0
    AND NoScan = 0;