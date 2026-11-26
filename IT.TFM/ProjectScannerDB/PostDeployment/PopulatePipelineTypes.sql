DECLARE @count08 INT
SELECT @count08 = COUNT(*) FROM dbo.PipelineTypes

IF @count08 = 0
BEGIN
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (1,  N'AzureFunction')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (2,  N'BatchConsole')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (3,  N'GenericJobs')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (4,  N'GenericSteps')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (5,  N'NugetPackage')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (6,  N'UniversalArtifact')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (7,  N'WebApp')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (8,  N'WinAppAppSync')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (9,  N'Angular')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (10, N'WinService')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (11, N'ApimApi')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (12, N'AppConfig')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (13, N'AzureFunctionNodeJs')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (14, N'BlobStorage')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (15, N'Kubernetes')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (16, N'SchemaRegistry')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (17, N'WebApiPython')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (18, N'WebAppPython')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (19, N'LegacyWebApp')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (20, N'LegacyAic')
END
