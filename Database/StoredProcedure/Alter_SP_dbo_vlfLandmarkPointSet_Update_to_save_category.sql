
/****** Object:  StoredProcedure [dbo].[vlfLandmarkPointSet_Update]    Script Date: 06/04/2015 3:47:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[vlfLandmarkPointSet_Update]
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
@pointSets varchar(max),
@createUserID int = -1,
@public bit = null,
@newCategoryId bigint = 0
AS

     declare @tblPointSets table
    (
		SequenceNum int IDENTITY(1,1),
		Latitude float,
		Longitude float,
		pointSets varchar(100)
    )

	if @createUserID = -1
	Begin
		select @createUserID = createUserId from dbo.vlfLandmark where OrganizationId=@organizationId  and LandmarkName = @currLandmarkName
	End

    insert into @tblPointSets(pointSets)
    select SplitValue from Split_Max(@pointSets,',')
	update @tblPointSets set 
	  Latitude = Substring(pointSets,1,Charindex('|',pointSets) -1),
	  Longitude = Substring(pointSets,Charindex('|',pointSets)+1,len(pointSets))

     Declare @ID bigint;
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
			   End

		IF (@newCategoryId <> -10)
			BEGIN
				
				Declare @CategoryMappingId bigint;

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

			END --(@newCategoryId <> -10)



		   
		Delete  vlfLandmarkPointSet where LandmarkId = @id
		Insert into vlfLandmarkPointSet(LandmarkId, SequenceNum, Latitude, Longitude)
		(Select @id as LandmarkId, SequenceNum, Latitude, Longitude from @tblPointSets)
	End
	
		UPDATE vlfOrganization set VirtualLandmarksModifiedDateTime = getutcdate() where organizationid = @OrganizationId

RETURN

GO


