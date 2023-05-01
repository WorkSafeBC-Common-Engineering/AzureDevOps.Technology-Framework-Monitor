DECLARE @count04 INT
SELECT @count04 = COUNT(*) FROM dbo.FilePropertyTypes

IF @count04 = 0
BEGIN
	INSERT INTO [dbo].[FilePropertyTypes] ([Id], [Value]) VALUES (1, N'Property')
	INSERT INTO [dbo].[FilePropertyTypes] ([Id], [Value]) VALUES (2, N'Filtered')
END