CREATE NONCLUSTERED INDEX [ReportsQueryIndex]
ON [dbo].[Reports] ([Organization],[ProjectId],[FrameworkVersion],[FileType])
INCLUDE ([Timestamp],[Project],[Repository],[RepositoryId],[FilePath],[IsEol],[EolDate],[Eol3Months],[Eol6Months],[Eol9Months],[Eol12Months],[SupportedFramework],[TargetFramework])