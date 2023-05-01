CREATE TABLE [dbo].[FileProperties]
(
	[Id] INT NOT NULL IDENTITY, 
	[PropertyTypeId] [int] NOT NULL,
    [FileId] INT NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_FileProperties] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_FileProperties_ToFiles] FOREIGN KEY ([FileId]) REFERENCES [dbo].[Files]([Id]) ,
    CONSTRAINT [FK_FileProperties_ToFilePropertyTypes] FOREIGN KEY([PropertyTypeId]) REFERENCES [dbo].[FilePropertyTypes] ([Id])
)
