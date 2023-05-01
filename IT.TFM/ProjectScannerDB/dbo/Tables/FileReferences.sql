CREATE TABLE [dbo].[FileReferences]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FileId] INT NOT NULL, 
    [FileReferenceTypeId] INT NOT NULL, 
    [PackageType] NVARCHAR(50) NULL, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Version] NVARCHAR(MAX) NULL, 
    [VersionComparator] NVARCHAR(5) NULL, 
    [FrameworkVersion] NVARCHAR(MAX) NULL, 
    [Path] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_FileReferences_ToFileReferenceTypes] FOREIGN KEY ([FileReferenceTypeId]) REFERENCES [dbo].[FileReferenceTypes]([Id]), 
    CONSTRAINT [FK_FileReferences_ToFiles] FOREIGN KEY ([FileId]) REFERENCES [dbo].[Files]([Id])
)
