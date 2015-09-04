USE [SentinelFM]
GO
/****** Object:  UserDefinedFunction [dbo].[udb_GetLastReeferAlarmExist]    Script Date: 09/16/2014 09:46:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Igor Avrutin
-- Create date: 2014-05-05
-- Description:	Returns last pre-trip state for last 7 days
-- =============================================
ALTER FUNCTION [dbo].[udb_GetLastReeferAlarmExist]
(
	-- Add the parameters for the function here
	@BoxId int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ret bit
	
	IF EXISTS(
	SELECT AlarmId 
	FROM dbo.vlfAlarm WITH(NOLOCK) 
	WHERE BoxId = @BoxId AND DateTimeCreated > dateadd(day,-7,GetUTCDate())
		AND AlarmType = 114 AND DateTimeClosed IS NULL
	)	
		SELECT @ret = 1
	ELSE
		SELECT @ret = 0
	-- Return the result of the function
	RETURN @ret

END