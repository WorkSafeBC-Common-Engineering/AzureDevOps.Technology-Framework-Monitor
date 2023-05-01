DECLARE @count05 INT
SELECT @count05 = COUNT(*) FROM dbo.FileReferenceTypes

IF @count05 = 0
BEGIN
	INSERT INTO [dbo].[FileReferenceTypes] ([Id], [Value]) VALUES (1, N'File')
	INSERT INTO [dbo].[FileReferenceTypes] ([Id], [Value]) VALUES (2, N'Url')
	INSERT INTO [dbo].[FileReferenceTypes] ([Id], [Value]) VALUES (3, N'Pkg')
END