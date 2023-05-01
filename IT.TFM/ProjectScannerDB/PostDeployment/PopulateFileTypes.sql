DECLARE @count06 INT
SELECT @count06 = COUNT(*) FROM dbo.FileTypes

IF @count06 = 0
BEGIN
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (1, N'NoMatch')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (2, N'VSSolution')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (3, N'CSProject')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (4, N'VBProject')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (5, N'SqlProject')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (6, N'VB6Project')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (7, N'VSConfig')
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (8, N'NuGetPkgConfig')	
	INSERT INTO [dbo].[FileTypes] ([Id], [Value]) VALUES (9, N'NpmPackage')
END