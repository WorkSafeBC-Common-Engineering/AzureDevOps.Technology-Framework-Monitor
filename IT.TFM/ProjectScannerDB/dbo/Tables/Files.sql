CREATE TABLE [dbo].[Files]
(
	[Id] INT NOT NULL IDENTITY,
    [RepositoryId] INT NOT NULL, 
    [FileTypeId] INT NOT NULL, 
    [FileId] NVARCHAR(50) NOT NULL, 
    [Path] NVARCHAR(MAX) NOT NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [CommitId] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_Files] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Files_ToRepositories] FOREIGN KEY ([RepositoryId]) REFERENCES [dbo].[Repositories]([Id]), 
    CONSTRAINT [FK_Files_ToFileTypeId] FOREIGN KEY ([FileTypeId]) REFERENCES [dbo].[FileTypes]([Id])
)

GO

CREATE FULLTEXT INDEX ON [dbo].[Files] ([FileId])
    KEY INDEX [PK_Files] ON [FilesFullTextCatalog] WITH CHANGE_TRACKING AUTO

GO

CREATE INDEX [IX_Files_RepositoryId] ON [dbo].[Files] ([RepositoryId])

GO

/****** Object:  Index [IX_FilesSearchSave]    Script Date: 2023/06/02 8:32:05 AM ******/
CREATE NONCLUSTERED INDEX [IX_FilesSearchSave] ON [dbo].[Files]
(
	[RepositoryId] ASC,
	[FileId] ASC
)
INCLUDE([Url]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

