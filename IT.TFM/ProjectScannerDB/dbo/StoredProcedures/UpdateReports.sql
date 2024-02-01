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
				CASE WHEN [IsAzureFunction] = 'Y' THEN 1 ELSE 0 END,
				[Package Version],
				CASE WHEN [Package - EOL] = 'YES' THEN 1 ELSE 0 END,
				[Package - Date EOL],
				CASE WHEN [Package EOL - 3 Months] = 'YES' THEN 1 ELSE 0 END,
				CASE WHEN [Package EOL - 6 Months] = 'YES' THEN 1 ELSE 0 END,
				CASE WHEN [Package EOL - 9 Months] = 'YES' THEN 1 ELSE 0 END,
				CASE WHEN [Package EOL - 12 Months] = 'YES' THEN 1 ELSE 0 END,
				[Is Supported Version],
				[Is Target Version],
				[File Reference],
				[URL Reference],
				[Pkg Type],
				[Pkg Reference],
				[Pkg Version]

	FROM		[dbo].[FullScan]
RETURN 0
