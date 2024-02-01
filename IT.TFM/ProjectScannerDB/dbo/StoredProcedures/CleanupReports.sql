CREATE PROCEDURE [dbo].[CleanupReports]
	@days int
AS
	DECLARE @oldDate DATE = DATEADD(DAY, -@days, GETDATE())

DELETE FROM [dbo].[Reports]
	WHERE [Timestamp] < @oldDate

RETURN 0
