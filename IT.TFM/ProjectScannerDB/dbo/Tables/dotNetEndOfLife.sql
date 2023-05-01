CREATE TABLE [dbo].[dotNetEndOfLife]
(
	[Version] NVARCHAR(20) NOT NULL PRIMARY KEY, 
    [EOL] DATE NOT NULL, 
    [ExtendedEOL] DATE NULL, 
    [Display] NVARCHAR(50) NOT NULL
)
