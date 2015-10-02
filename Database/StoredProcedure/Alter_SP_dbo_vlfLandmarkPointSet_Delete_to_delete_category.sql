USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[vlfLandmarkPointSet_Delete]    Script Date: 12/03/2015 4:30:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Alter PROCEDURE [dbo].[vlfLandmarkPointSet_Delete]  
@organizationId int,  
@LandmarkName varchar(100)  
AS  
  
	 Declare @CategoryMappingId bigint;
     Declare @ID bigint;  
  Select top 1 @ID = LandmarkId from dbo.vlfLandmark where OrganizationId=@organizationId  and LandmarkName = @LandmarkName  
  
  if @ID is not null  
  Begin  
       Delete  vlfLandmarkPointSet where LandmarkId = @ID  
       Delete dbo.vlfLandmark   
        where OrganizationId=@organizationId  and LandmarkName = @LandmarkName  
  
     delete vlfFleetObjects where ObjectName='landmark' and ObjectId = @ID  
  delete vlfServiceAssignment where LandmarkId = @ID  
  

		-- Begins: Delete Landmark category
		Set @CategoryMappingId = 0;

		Select @CategoryMappingId = DMM.DomainMetadataMappingId
		From dbo.DomainMetadataMapping DMM (NOLOCK)
		Where DMM.RecordId = @ID AND DMM.DomainMetadataId IN 
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
		-- Ends: Delete Landmark category
  
 End  
   
  UPDATE vlfOrganization set VirtualLandmarksModifiedDateTime = getutcdate() where organizationid = @organizationId  
  
RETURN  
GO


