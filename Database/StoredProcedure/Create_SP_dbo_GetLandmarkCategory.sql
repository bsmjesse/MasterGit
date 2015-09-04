
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[GetLandmarkCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetLandmarkCategory]
GO


CREATE PROCEDURE [dbo].[GetLandmarkCategory]
@OrganizationId int,
@UserId int,
@LandmarkId bigint


 AS 

 Select DMM.DomainMetadataMappingId as LandmarkCategoryMappingId, 
		DMM.DomainMetadataId as LandmarkCategoryId, 
		DMM.RecordId as LandmarkId,
		LandmarkCategoryName = (Select MetadataValue From dbo.DomainMetadata Where DomainMetadataId=DMM.DomainMetadataId)
 From dbo.DomainMetadataMapping DMM (NOLOCK)
 Where DMM.RecordId=@LandmarkId AND DMM.DomainMetadataId IN 
							(
								Select DM.DomainMetadataId 
								From dbo.DomainMetadata DM (NOLOCK)
								Where DM.DomainId=1 AND DM.OrganizationId=@OrganizationId
							)

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

