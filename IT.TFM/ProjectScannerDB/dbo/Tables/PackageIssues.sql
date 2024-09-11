CREATE TABLE [dbo].[PackageIssues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScanType] [nvarchar](20) NOT NULL,
	[FileId] [int] NOT NULL,
	[Framework] [nvarchar](50) NOT NULL,
	[IsTopLevel] [bit] NOT NULL,
	[PackageName] [nvarchar](max) NOT NULL,
	[RequestedVersion] [nvarchar](50) NULL,
	[ResolvedVersion] [nvarchar](50) NULL,
	[LatestVersion] [nvarchar](50) NULL,
	[Severity] [nvarchar](50) NULL,
	[AdvisoryUrl] [nvarchar](max) NULL, 
    CONSTRAINT [FK_PackageIssues_ToFiles] FOREIGN KEY (FileId) REFERENCES [dbo].[Files](Id)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PackageIssues] ADD  CONSTRAINT [DF_PackageIssues_IsTopLevel]  DEFAULT ((0)) FOR [IsTopLevel]
GO
