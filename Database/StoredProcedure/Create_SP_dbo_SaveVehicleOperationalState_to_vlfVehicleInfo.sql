

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[SaveVehicleOperationalState]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveVehicleOperationalState]
GO


CREATE PROCEDURE [dbo].[SaveVehicleOperationalState]
@OrganizationId int,
@UserId int,
@VehicleId bigint,
@OperationalState int,
@Notes nvarchar(250)

 AS 
 
	--Update
	Update dbo.vlfVehicleInfo 
		Set OperationalState = @OperationalState,
			Notes = @Notes
	Where VehicleId=@VehicleId And OrganizationId=@OrganizationId
	
	
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


