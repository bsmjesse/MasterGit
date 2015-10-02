

GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[sp_vlfGetServiceConfigurationsByLandmarkAndVehicle]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[sp_vlfGetServiceConfigurationsByLandmarkAndVehicle]
GO


CREATE PROCEDURE [dbo].[sp_vlfGetServiceConfigurationsByLandmarkAndVehicle]
      -- Add the parameters for the stored procedure here
      @organizationId AS INT,
      @vehicleId as bigint,
      @landmarkId as bigint
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;
      SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
      DECLARE @t table (f int)
      INSERT INTO @t (f) 
      SELECT FleetId from vlfFleetVehicles WHERE VehicleId=@vehicleId


	  Select DISTINCT LSC.ServiceConfigID, LSC.ServiceConfigName --, LSC.LandmarkID, VSC.Objects, VSC.ObjectID
	  From 
		  (
			  SELECT vc.*, va.ObjectID as LandmarkID 
			  FROM vlfServiceAssignments va
					INNER JOIN vlfServiceConfigurations vc ON va.ServiceConfigID=vc.ServiceConfigID 
			  WHERE 
				  vc.OrganizationID = @organizationId
				  AND vc.ExpiredDate IS NULL
				  AND vc.IsActive = 1
				  AND (va.Objects='Landmark' AND va.ObjectID=@landmarkId)
				  AND va.Deleted=0
			) LSC
			INNER JOIN 
				(

					  SELECT vc.*, va.Objects, va.ObjectID 
					  FROM vlfServiceAssignments va
							INNER JOIN vlfServiceConfigurations vc ON va.ServiceConfigID=vc.ServiceConfigID 
					  WHERE 
						  vc.OrganizationID = @organizationId
						  AND vc.ExpiredDate IS NULL
						  AND vc.IsActive = 1
						  AND
						  (
							  (
									(va.Objects='Fleet' AND va.ObjectID IN (SELECT f from @t)) 
									OR 
									(va.Objects='Vehicle' AND va.ObjectID=@vehicleId)
							  ) 
						  ) 
						  AND @vehicleId NOT IN (
													SELECT ObjectID 
													FROM vlfServiceAssignments va1 
													WHERE va1.ServiceConfigID=va.ServiceConfigID AND va1.Objects='Vehicle' AND va1.Inclusive=0
												) 
						  AND va.Deleted=0
					) VSC ON LSC.ServiceConfigID=VSC.ServiceConfigID
	  Where LSC.RulesApplied LIKE '%PostponeCount%'

END


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




 