CREATE TABLE [dbo].[NuGetFeeds]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL,
    [Url] NVARCHAR(MAX) NOT NULL, 
    [FeedUrl] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [ProjectId] INT NULL, 
    CONSTRAINT [FK_NuGetFeeds_ToProjects] FOREIGN KEY ([ProjectId]) REFERENCES [Projects]([Id]) 
)
