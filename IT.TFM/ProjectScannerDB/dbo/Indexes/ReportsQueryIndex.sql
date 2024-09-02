CREATE NONCLUSTERED INDEX [ReportsQueryIndex] ON [dbo].[Reports]
(
	[Timestamp] ASC,
	[Organization] ASC,
	[ProjectId] ASC,
	[FrameworkVersion] ASC,
	[PackageName] ASC,
	[FileType] ASC
)
INCLUDE([Project],[Repository],[RepositoryId],[FilePath],[IsEol],[EolDate],[Eol3Months],[Eol6Months],[Eol9Months],[Eol12Months],[SupportedFramework],[TargetFramework]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
