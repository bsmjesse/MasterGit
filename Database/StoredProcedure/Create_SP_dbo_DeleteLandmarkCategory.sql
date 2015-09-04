
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[DeleteLandmarkCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteLandmarkCategory]
GO

CREATE PROCEDURE [dbo].[DeleteLandmarkCategory]
@OrganizationId int,
@UserId int,
@LandmarkId bigint

 AS 

Declare @CategoryMappingId bigint;
Set @CategoryMappingId = 0;

Select @CategoryMappingId = DMM.DomainMetadataMappingId
From dbo.DomainMetadataMapping DMM (NOLOCK)
Where DMM.RecordId = @LandmarkId AND DMM.DomainMetadataId IN 
							(
								Select DM.DomainMetadataId 
								From dbo.DomainMetadata DM (NOLOCK)
								Where DM.DomainId=1 AND DM.OrganizationId=@organizationId
							)

IF (@CategoryMappingId > 0)
	BEGIN
		--Delete
		Delete From dbo.DomainMetadataMapping 
		Where DomainMetadataMappingId=@CategoryMappingId 
	END


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

