
/****** Object:  StoredProcedure [dbo].[vlfLandmarkMetaData_Update]    Script Date: 09/03/2015 1:18:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[vlfLandmarkMetaData_Update]
@organizationId int,
@currLandmarkName varchar(100),
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
@public bit = null,
@newCategoryId bigint = 0
AS

     Declare @ID bigint;
	 Declare @CategoryMappingId bigint;
	 Select top 1 @ID = LandmarkId from dbo.vlfLandmark where OrganizationId=@organizationId  and LandmarkName = @currLandmarkName

	 if @id is not null
	 Begin
			if @public is not null
			Begin
				 Update dbo.vlfLandmark Set
									LandmarkName = @LandmarkName
									,Latitude = @Latitude
									,Longitude = @Longitude
									,Description = @Description
									,ContactPersonName = @ContactPersonName
									,ContactPhoneNum = @ContactPhoneNum
									,Radius = @Radius
									,Email = @Email
									,phone = @phone
									,TimeZone = @TimeZone
									,DayLightSaving = @DayLightSaving
									,AutoAdjustDayLightSaving = @AutoAdjustDayLightSaving
									,StreetAddress = @StreetAddress
									,CreateUserID = @createUserID
									,[Public] = @public
									,UpdatedDateTime = GETDATE()

									where OrganizationId=@organizationId  and LandmarkName = @currLandmarkName

					
				 IF (@newCategoryId > 0)
				 BEGIN

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
							--Update
							Update dbo.DomainMetadataMapping 
								Set DomainMetadataId = @newCategoryId 
							Where DomainMetadataMappingId=@CategoryMappingId 
						END
					ELSE
						BEGIN
							--Insert
							INSERT dbo.DomainMetadataMapping (DomainMetadataId, RecordId) Values (@newCategoryId, @ID)
						END --(@CategoryMappingId > 0)

				 END --(@newCategoryId > 0)
			
			   end
			   Else
			   Begin
					Update dbo.vlfLandmark Set
									LandmarkName = @LandmarkName
									,Latitude = @Latitude
									,Longitude = @Longitude
									,Description = @Description
									,ContactPersonName = @ContactPersonName
									,ContactPhoneNum = @ContactPhoneNum
									,Radius = @Radius
									,Email = @Email
									,phone = @phone
									,TimeZone = @TimeZone
									,DayLightSaving = @DayLightSaving
									,AutoAdjustDayLightSaving = @AutoAdjustDayLightSaving
									,StreetAddress = @StreetAddress
									,CreateUserID = @createUserID
									,UpdatedDateTime = GETDATE()								

									where OrganizationId=@organizationId  and LandmarkName = @currLandmarkName


					IF (@newCategoryId > 0)
					 BEGIN

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
								--Update
								Update dbo.DomainMetadataMapping 
									Set DomainMetadataId = @newCategoryId 
								Where DomainMetadataMappingId=@CategoryMappingId 
							END
						ELSE
							BEGIN
								--Insert
								INSERT dbo.DomainMetadataMapping (DomainMetadataId, RecordId) Values (@newCategoryId, @ID)
							END --(@CategoryMappingId > 0)

					 END --(@newCategoryId > 0)

			   End
		   
		
	End
	
	UPDATE vlfOrganization set VirtualLandmarksModifiedDateTime = getutcdate() where organizationid = @OrganizationId

RETURN

GO


