

/****** Object:  StoredProcedure [dbo].[vlfLandmarkPointSet_Add]    Script Date: 20/03/2015 2:10:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[vlfLandmarkPointSet_Add]
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
@pointSets varchar(max),
@createUserID int = -1,
@public bit = 1,
@newCategoryId bigint = 0
AS

     declare @tblPointSets table
    (
		SequenceNum int IDENTITY(1,1),
		Latitude float,
		Longitude float,
		pointSets varchar(100)
    )

    insert into @tblPointSets(pointSets)
    select SplitValue from Split_Max(@pointSets,',')
	update @tblPointSets set 
	  Latitude = Substring(pointSets,1,Charindex('|',pointSets) -1),
	  Longitude = Substring(pointSets,Charindex('|',pointSets)+1,len(pointSets))


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
	Insert into vlfLandmarkPointSet(LandmarkId, SequenceNum, Latitude, Longitude)
	(Select @id as LandmarkId, SequenceNum, Latitude, Longitude from @tblPointSets)

	IF (@newCategoryId > 0)
		BEGIN
			--Insert
			INSERT dbo.DomainMetadataMapping (DomainMetadataId, RecordId) Values (@newCategoryId, @id)
		END --(@newCategoryId > 0)


	UPDATE vlfOrganization set VirtualLandmarksModifiedDateTime = getutcdate() where organizationid = @OrganizationId

RETURN

GO





