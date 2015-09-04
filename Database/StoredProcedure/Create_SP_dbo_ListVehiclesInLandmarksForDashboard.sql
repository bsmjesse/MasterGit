

GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[ListVehiclesInLandmarksForDashboard]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ListVehiclesInLandmarksForDashboard]
GO


CREATE PROCEDURE [dbo].[ListVehiclesInLandmarksForDashboard]
@OrganizationId int,
@UserId int=0,
@LandmarkCategoryId int
 AS 

Select X.*, 
	IsNull(VI.OperationalState, 100) As OperationalState, 
	VI.Notes AS OperationalStateNotes, 
	L.LandmarkName 
From
	(
		Select E.ID, E.BoxID, E.VehicleID, E.LandmarkID, E.StDate as LandmarkInDateTime, 
						DATEDIFF(minute, E.StDate, GETUTCDATE()) As DurationInLandmarkMin,
						E.Notes as EventNotes
		From evtEventsLast E (NOLOCK) 
		Where E.OrganizationID=@OrganizationId And E.EventID=20 And IsNull(E.IsExpired,0)=0 And E.StDate=E.EndDate
	) X
	INNER JOIN vlfVehicleInfo VI (NOLOCK) ON X.VehicleID=VI.VehicleId
	INNER JOIN (Select DMM.RecordId From DomainMetadataMapping DMM Where DMM.DomainMetadataId=@LandmarkCategoryId) D ON X.LandmarkID=D.RecordId
	INNER JOIN vlfLandmark L (NOLOCK) ON X.LandmarkID=L.LandmarkId
ORDER By X.VehicleID ASC


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 

