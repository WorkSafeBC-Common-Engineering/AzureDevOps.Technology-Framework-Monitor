DECLARE @count08 INT
SELECT @count08 = COUNT(*) FROM dbo.PipelineTypes

IF @count08 = 0
BEGIN
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (1, N'AzureFunction')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (2, N'BatchConsole')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (3, N'GenericJobs')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (4, N'GenericSteps')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (5, N'NugetPackage')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (6, N'UniversalArtifact')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (7, N'WebApp')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (8, N'WinAppAppSync')
END