
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[usp_LandmarkInOut_Update_EndDate_Of_UnavailableEvents]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[usp_LandmarkInOut_Update_EndDate_Of_UnavailableEvents]
GO

-- =============================================
-- Author:		Subas
-- Create date: 2015-09-17
-- Description:	Update EventID=200 record with Duration, EndDateTime, IsEvent fields
-- History:
--		- 2015-09-21 - Created
--		- 
-- =============================================

CREATE PROCEDURE [dbo].[usp_LandmarkInOut_Update_EndDate_Of_UnavailableEvents] 
	-- Add the parameters for the stored procedure here
	@organizationId INT,
	@userId BIGINT = 0,
	@boxId INT,
	@landmarkId INT,
	@LandmarkInDate DATETIME,
	@makeItUnavailableAgain BIT
AS
BEGIN

	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	BEGIN TRY					
		DECLARE @pEndDate DATETIME;	
		DECLARE @pUnavailabeEventRecordId BIGINT;
		DECLARE @pSearchFromDate DATETIME;	
		
		SET @pEndDate = GETUTCDATE();
		SET @pSearchFromDate = DATEADD(HOUR, -48, @LandmarkInDate);

		SELECT @pUnavailabeEventRecordId = ID
		FROM dbo.evtEvents WITH (NOLOCK) 
		WHERE EventID=200 AND BoxID=@boxId AND OrganizationID=@organizationId AND 
				LandmarkID=@landmarkId AND ISNULL(IsExpired,0)=0 AND StDate > @pSearchFromDate;	
			
		IF @pUnavailabeEventRecordId > 0 
			BEGIN

				IF @makeItUnavailableAgain = 0
					BEGIN
						UPDATE dbo.evtEvents 
							SET 
									EndDate = @pEndDate, 
									Duration = DATEDIFF(SECOND,StDate,@pEndDate),
									IsEvent = 1
						WHERE ID = @pUnavailabeEventRecordId AND EventID=200;
					END
				ELSE
					BEGIN
						UPDATE dbo.evtEvents 
							SET		IsEvent = 0
						WHERE ID = @pUnavailabeEventRecordId AND EventID=200;
					END
			END
				
	END TRY
	BEGIN CATCH
		INSERT INTO ErrorLog(Boxid, CustomProp, ErrorDesc, OriginDT, [Object])
                VALUES(@boxId, '', Error_Message(), @pEndDate, 'usp_LandmarkInOut_Update_EndDate_Of_UnavailableEvents')
	END CATCH

END

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


