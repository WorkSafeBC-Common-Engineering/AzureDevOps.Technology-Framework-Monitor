CREATE TABLE [dbo].[Pipelines]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineId] INT NOT NULL, 
    [RepositoryId] INT NOT NULL, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Folder] NVARCHAR(MAX) NOT NULL, 
    [Revision] INT NOT NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [Type] NVARCHAR(20) NOT NULL, 
    [PipelineType] NVARCHAR(20) NOT NULL, 
    [QueueStatus] NVARCHAR(20) NOT NULL, 
    [Quality] NVARCHAR(20) NOT NULL, 
    [CreatedBy] NVARCHAR(100) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    CONSTRAINT [FK_Pipelines_ToRepository] FOREIGN KEY ([RepositoryId]) REFERENCES Repositories([Id]) 
)
