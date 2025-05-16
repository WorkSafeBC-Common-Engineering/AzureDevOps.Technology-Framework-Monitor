DECLARE @count08 INT
SELECT @count08 = COUNT(*) FROM dbo.PipelineTypes

IF @count08 = 0
BEGIN
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (1, N'Azure-Function')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (2, N'Batch-Console')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (3, N'Generic-Jobs')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (4, N'Generic-Steps')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (5, N'Nuget-Package')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (6, N'Universal-Artifact')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (7, N'WebApp')
	INSERT INTO [dbo].[PipelineTypes] ([Id], [Value]) VALUES (8, N'WinApp-AppSync')
END