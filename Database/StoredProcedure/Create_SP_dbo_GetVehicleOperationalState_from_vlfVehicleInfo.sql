
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[GetVehicleOperationalState]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetVehicleOperationalState]
GO


CREATE PROCEDURE [dbo].[GetVehicleOperationalState]
@OrganizationId int,
@UserId int,
@VehicleId bigint


 AS 
 
 Select VI.VehicleId, VI.Description, VI.OperationalState, VI.Notes 
	From vlfVehicleInfo VI (NOLOCK)
	Where VI.VehicleId=@VehicleId And VI.OrganizationId=@OrganizationId

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

