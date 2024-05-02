﻿CREATE VIEW [dbo].[ReportScan]
	AS
SELECT		MAX(Rx.Timestamp) AS [Timestamp],
			Rx.Organization AS [Organization],
			Rx.[ProjectId] AS [ProjectId],
			Rx.[Project] AS [ProjectName],
			Rx.[RepositoryId] AS [RepositoryId],
			Rx.[Repository] AS [RepositoryName],
			Rx.[FileType] AS [FileType],
			Rx.[FileId] AS [FileId],
			Rx.[FilePath] AS [FilePath],
			Rx.AzureFunction AS AzureFunctionRuntime,
			Rx.IsAzureFunction AS IsAzureFunction,
			Rx.FrameworkVersion AS [FrameworkVersion],
			Rx.IsEol AS [IsEol],
			Rx.EolDate AS [EolDate],
			Rx.Eol3Months AS [Eol3Months],
			Rx.Eol6Months AS [Eol6Months],
			Rx.Eol9Months AS [Eol9Months],
			Rx.Eol12Months AS [Eol12Months],
			Rx.SupportedFramework AS [SupportedFramework],
			Rx.TargetFramework AS [TargetFramework],
			Rx.FileReference AS [FileReference],
			Rx.UrlReference AS [UrlReference],
			Rx.PackageType AS [PackageType],
			Rx.PackageName AS [PackageReference],
			Rx.PackageVersion AS [PackageVersion]

FROM		Reports Rx

GROUP BY	Rx.Organization,
			Rx.[ProjectId],
			Rx.[Project],
			Rx.[RepositoryId],
			Rx.[Repository],
			Rx.[FileType],
			Rx.[FileId],
			Rx.[FilePath],
			Rx.AzureFunction,
			Rx.IsAzureFunction,
			Rx.FrameworkVersion,
			Rx.IsEol,
			Rx.EolDate,
			Rx.Eol3Months,
			Rx.Eol6Months,
			Rx.Eol9Months,
			Rx.Eol12Months,
			Rx.SupportedFramework,
			Rx.TargetFramework,
			Rx.FileReference,
			Rx.UrlReference,
			Rx.PackageType,
			Rx.PackageName,
			Rx.PackageVersion
