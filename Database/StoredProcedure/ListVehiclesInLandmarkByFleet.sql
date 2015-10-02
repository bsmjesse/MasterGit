USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[ListVehiclesInLandmarkByFleet]    Script Date: 09/21/2015 10:32:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ListVehiclesInLandmarkByFleet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ListVehiclesInLandmarkByFleet]
GO

USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[ListVehiclesInLandmarkByFleet]    Script Date: 09/21/2015 10:32:11 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[ListVehiclesInLandmarkByFleet]
@UserId int,
@OrganizationId int,
@FleetId int = 0,
@LandmarkId bigint
 AS 



		Select DISTINCT E.ID, E.BoxID, E.VehicleID, E.LandmarkID, E.StDate as LandmarkInDateTime, 
						DATEDIFF(minute, E.StDate, GETUTCDATE()) As DurationInLandmarkMin,
						E.Notes as EventNotes
		From evtEventsLast E WITH(NOLOCK) 
		INNER JOIN vlfVehicleAssignment VA WITH(NOLOCK) ON E.VehicleID=VA.VehicleId AND E.BoxID=VA.BoxId
		Where E.OrganizationID=@OrganizationId AND E.LandmarkID=@LandmarkId And E.EventID=20 And IsNull(E.IsExpired,0)=0 And E.StDate=E.EndDate
	


SET QUOTED_IDENTIFIER OFF 


GO


