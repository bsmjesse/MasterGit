USE [SentinelFM]
GO

 Object  StoredProcedure [dbo].[GetVehicleAvailabilityByManagerForDashboard]    Script Date 09082015 140208 
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Subas
-- =============================================

CREATE PROCEDURE [dbo].[GetVehicleAvailabilityByManagerForDashboard]
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

	SELECT ManagerName, COUNT() AS Total, 
		SUM(Available) as NumberOfAvailable,
		(COUNT() -  SUM(Available)) AS NumberOfUnavailable
	FROM Vehicle_Availability
	GROUP By ManagerName
	ORDER By ManagerName ASC


SET QUOTED_IDENTIFIER OFF 

GO


