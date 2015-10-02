
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[SaveLandmarkCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveLandmarkCategory]
GO


CREATE PROCEDURE [dbo].[SaveLandmarkCategory]
@OrganizationId int,
@UserId int,
@LandmarkId bigint,
@CategoryId bigint

 AS 

Declare @CategoryMappingId bigint;
Set @CategoryMappingId = 0;

IF (@CategoryId > 0)
BEGIN

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
			--Update
			Update dbo.DomainMetadataMapping 
				Set DomainMetadataId = @CategoryId 
			Where DomainMetadataMappingId=@CategoryMappingId 
		END
	ELSE
		BEGIN
			--Insert
			INSERT INTO dbo.DomainMetadataMapping (DomainMetadataId, RecordId) Values (@CategoryId, @LandmarkId)
		END --(@CategoryMappingId > 0)

END --(@CategoryId > 0)
			

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

