CREATE TABLE [dbo].[Files]
(
	[Id] INT NOT NULL IDENTITY,
    [RepositoryId] INT NOT NULL, 
    [FileTypeId] INT NOT NULL, 
    [FileId] NVARCHAR(MAX) NOT NULL, 
    [Path] NVARCHAR(MAX) NOT NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [SHA1] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_Files] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Files_ToRepositories] FOREIGN KEY ([RepositoryId]) REFERENCES [dbo].[Repositories]([Id]), 
    CONSTRAINT [FK_Files_ToFileTypeId] FOREIGN KEY ([FileTypeId]) REFERENCES [dbo].[FileTypes]([Id])
)

GO

CREATE FULLTEXT INDEX ON [dbo].[Files] ([FileId])
    KEY INDEX [PK_Files] ON [FilesFullTextCatalog] WITH CHANGE_TRACKING AUTO

GO

CREATE INDEX [IX_Files_RepositoryId] ON [dbo].[Files] ([RepositoryId])
