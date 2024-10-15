CREATE TABLE [dbo].[NugetTargetFrameworks]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [NuGetPackageId] INT NOT NULL, 
    [Framework] NVARCHAR(50) NOT NULL, 
    [Version] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_NugetTargetFrameworks_ToNuGetPackages] FOREIGN KEY ([NuGetPackageId]) REFERENCES [NuGetPackages]([Id])
)
