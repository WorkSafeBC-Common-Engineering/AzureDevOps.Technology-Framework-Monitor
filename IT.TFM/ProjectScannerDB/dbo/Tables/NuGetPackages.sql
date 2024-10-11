CREATE TABLE [dbo].[NuGetPackages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RepositoryId] INT NULL,
    [NuGetFeedId] INT NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Version] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [Authors] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [Published] DATETIME NOT NULL, 
    [ProjectUrl] NVARCHAR(MAX) NULL, 
    [Tags] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    CONSTRAINT [FK_NuGetPackages_ToRepositories] FOREIGN KEY ([RepositoryId]) REFERENCES [Repositories]([Id])
)
