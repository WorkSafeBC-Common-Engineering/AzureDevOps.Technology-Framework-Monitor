CREATE TABLE [dbo].[FrameworkProducts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [ProductName] NVARCHAR(50) NOT NULL, 
    [Source] NVARCHAR(50) NULL, 
    [SNOWReferenceId] NVARCHAR(50) NULL
)
