
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[vlfLandmark_Add_ForImport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[vlfLandmark_Add_ForImport]
GO

CREATE PROCEDURE [dbo].[vlfLandmark_Add_ForImport]
@organizationId int,
@landmarkName varchar(100),
@latitude float,
@longitude float,
@description varchar(100), 
@contactPersonName varchar(100),
@contactPhoneNum varchar(20),
@radius int, 
@email varchar(128),
@phone varchar(55),
@timeZone int, 
@dayLightSaving bit, 
@autoAdjustDayLightSaving bit,
@streetAddress varchar(256),
@createUserID int = -1,
@public bit = 1,
@categoryName nvarchar(40)
AS

     INSERT INTO dbo.vlfLandmark( 
                        OrganizationId
                        ,LandmarkName
                        ,Latitude
                        ,Longitude
                        ,Description
                        ,ContactPersonName
                        ,ContactPhoneNum
                        ,Radius
                        ,Email
						,phone
						,TimeZone
						,DayLightSaving
						,AutoAdjustDayLightSaving
						,StreetAddress
						,CreateUserID
						,[Public]) VALUES ( 
						@OrganizationId,
						@LandmarkName,
						@Latitude,
						@Longitude,
						@Description,
						@ContactPersonName,
						@ContactPhoneNum,
						@Radius,@Email,
						@phone,
						@TimeZone,
						@DayLightSaving,
						@AutoAdjustDayLightSaving,
						@StreetAddress,
						@createUserID,
						@public )
    	
	Declare @id bigint;
    Set @id = SCOPE_IDENTITY()
	
	Declare @categoryId bigint;
	Set @categoryId = 0;
	
	Select @categoryId = DomainMetadataId 
	From DomainMetadata DM (NOLOCK) 
	Where DM.MetadataValue=@categoryName And DM.DomainId=1 And DM.OrganizationId=@organizationId 
	
	IF (@categoryId = 0)
		BEGIN
			DECLARE @InsertedDetails TABLE (CategoryID bigint);
			
			-- Insert metadata
			 INSERT INTO dbo.DomainMetadata (OrganizationId, DomainId, MetadataValue) 
				OUTPUT inserted.DomainMetadataId INTO @InsertedDetails
			 Values (@organizationId, 1, @categoryName)
			 
			 Select @categoryId = CategoryID From @InsertedDetails  
		END 

	-- Associate Category to Landmark
	INSERT dbo.DomainMetadataMapping (DomainMetadataId, RecordId) Values (@categoryId, @id)

	UPDATE vlfOrganization set VirtualLandmarksModifiedDateTime = getutcdate() where organizationid = @OrganizationId

RETURN

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


