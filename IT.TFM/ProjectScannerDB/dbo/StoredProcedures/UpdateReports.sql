CREATE PROCEDURE [dbo].[UpdateReports]
AS
	DECLARE @timestamp DATE = GETDATE()


	INSERT INTO dbo.Reports (Timestamp, Organization, Project, ProjectId, ProjectDescription, ProjectUrl,
							 Repository, RepositoryId, RepositoryDefaultBranch, RepositoryUrl,
	                         FileType, FileId, FilePath, FileUrl,
							 AzureFunction, IsAzureFunction, FrameworkVersion, IsEol, EolDate, Eol3Months, Eol6Months, Eol9Months, Eol12Months,
							 SupportedFramework, TargetFramework, FileReference, UrlReference, PackageType, PackageName, PackageVersion)							 

	SELECT		@timestamp,
				[Org. Name],
				[Project Name],
				[Project ID],
				[Project Description],
				[Project Url],
				[Repository Name],
				[Repository ID],
				[Repository Default Branch],
				[Repository URL],
				[File Type],
				[File ID],
				[File Path],
				[File URL],
				[PROP_AzureFunction],
				[IsAzureFunction],
				[Package Version],
				[Package - EOL],
				[Package - Date EOL],
				[Package EOL - 3 Months],
				[Package EOL - 6 Months],
				[Package EOL - 9 Months],
				[Package EOL - 12 Months],
				[Is Supported Version],
				[Is Target Version],
				[File Reference],
				[URL Reference],
				[Pkg Type],
				[Pkg Reference],
				[Pkg Version]

	FROM		[dbo].[FullScan]
RETURN 0
