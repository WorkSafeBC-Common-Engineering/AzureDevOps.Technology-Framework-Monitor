CREATE TABLE [dbo].[dotNetEndOfLife]
(
	[Version] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [EOL] DATE NULL, 
    [ExtendedEOL] DATE NULL, 
    [Display] NVARCHAR(50) NOT NULL, 
    [IsTargetVersion] BIT NOT NULL DEFAULT 0
)
