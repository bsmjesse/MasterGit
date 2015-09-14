
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[GetVehicleAvailabilityByManagerForDashboard]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetVehicleAvailabilityByManagerForDashboard]
GO


CREATE PROCEDURE [GetVehicleAvailabilityByManagerForDashboard]
@OrganizationId int,
@UserId int=0,
@FleetId int
 AS 

	WITH  Vehicle_Availability AS
	(
		SELECT VI.ManagerName,
				CASE 
					WHEN ISNULL(VI.OperationalState,100)=100 THEN 1 
					ELSE 0 
				END AS Available
		FROM vlfVehicleInfo VI (NOLOCK) 
			INNER JOIN vlfFleetVehicles FV (NOLOCK) ON VI.VehicleId=FV.VehicleId
			INNER JOIN vlfVehicleAssignment VA (NOLOCK) ON VI.VehicleID=VA.VehicleId
		WHERE FV.FleetId=@FleetId AND VI.OrganizationId=@OrganizationId
	)

	SELECT ManagerName, COUNT(*) AS Total, 
		SUM(Available) as NumberOfAvailable,
		(COUNT(*) -  SUM(Available)) AS NumberOfUnavailable
	FROM Vehicle_Availability
	GROUP By ManagerName
	ORDER By ManagerName ASC


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




