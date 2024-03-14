CREATE TABLE [dbo].[Pipelines]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineId] INT NULL, 
    [RepositoryId] INT NULL, 
    [FileId] INT NULL,
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Folder] NVARCHAR(MAX) NOT NULL, 
    [Revision] INT NOT NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [Type] NVARCHAR(20) NOT NULL, 
    [PipelineType] NVARCHAR(20) NULL, 
    [Path] NVARCHAR(MAX) NULL,
    [YamlType] NVARCHAR(20) NULL,
    [Portfolio] NVARCHAR(50) NULL,
    [Product] NVARCHAR(50) NULL,
    CONSTRAINT [FK_Pipelines_ToRepository] FOREIGN KEY ([RepositoryId]) REFERENCES Repositories([Id]), 
    CONSTRAINT [FK_Pipelines_ToFile] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id])
)
