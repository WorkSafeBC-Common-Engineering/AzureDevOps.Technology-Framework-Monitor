CREATE TABLE [dbo].[dotNetEndOfLife]
(
	[Version] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [EOL] DATE NULL, 
    [ExtendedEOL] DATE NULL, 
    [Display] NVARCHAR(50) NOT NULL, 
    [IsTargetVersion] BIT NOT NULL DEFAULT 0, 
    [ReleaseDate] DATE NULL,
    [FrameworkProductId] INT NULL, 
    CONSTRAINT [FK_dotNetEndOfLife_ToFrameworkProduct] FOREIGN KEY ([FrameworkProductId]) REFERENCES [FrameworkProducts]([Id])
)
