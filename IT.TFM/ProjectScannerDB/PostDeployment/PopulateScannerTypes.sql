DECLARE @count07 INT
SELECT @count07 = COUNT(*) FROM dbo.ScannerTypes

IF @count07 = 0
BEGIN
	INSERT INTO [dbo].[ScannerTypes] ([Id], [Value]) VALUES (1, N'AzureDevOps')
END