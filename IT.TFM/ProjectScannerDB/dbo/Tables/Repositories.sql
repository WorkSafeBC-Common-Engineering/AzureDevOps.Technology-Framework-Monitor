CREATE TABLE [dbo].[Repositories]
(
	[Id] INT NOT NULL IDENTITY,
    [ProjectId] INT NOT NULL, 
    [RepositoryId] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Portfolio] NVARCHAR(MAX) NULL,
    [ApplicationProjectName] NVARCHAR(MAX) NULL,
    [ComponentName] NVARCHAR(MAX) NULL,
    [DefaultBranch] NVARCHAR(MAX) NULL, 
    [IsFork] BIT NOT NULL DEFAULT 0, 
    [Size] BIGINT NOT NULL DEFAULT 0, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [RemoteUrl] NVARCHAR(MAX) NULL, 
    [WebUrl] NVARCHAR(MAX) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0,
    [LastCommitId] NVARCHAR(50) NULL DEFAULT '', 
    [TooBig] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Repositories] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Repositories_ToProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([Id])
)

GO

CREATE INDEX [IX_Repositories_RepositoryId] ON [dbo].[Repositories] ([RepositoryId])
