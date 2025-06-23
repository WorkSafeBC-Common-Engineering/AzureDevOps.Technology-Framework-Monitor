CREATE VIEW [analysis].dimFiles AS
SELECT  f.Id AS FileId,
        f.Path AS FilePath,
        f.Url AS FileUrl,
        ft.Value AS FileType,
        f.RepositoryId
FROM    dbo.Files f
INNER JOIN dbo.FileTypes ft ON f.FileTypeId = ft.Id;
