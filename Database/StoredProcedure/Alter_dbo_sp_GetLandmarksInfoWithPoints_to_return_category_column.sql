
/****** Object:  StoredProcedure [dbo].[sp_GetLandmarksInfoWithPoints]    Script Date: 20/03/2015 12:27:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[sp_GetLandmarksInfoWithPoints]  
@organizationId int,  
@userId int = 0 ,
@categoryId bigint = 0 
AS  
BEGIN  
  
	SET ARITHABORT ON   
	SET QUOTED_IDENTIFIER ON  
	DECLARE @UserGroupId INT  

	IF @organizationId = 1000051 ---LIRR - Landmarks and Geofences to be private for all except administrators
	   BEGIN
			
		    DECLARE  @UserGroupId_Parent int, @UserGroupId_std int
			set @UserGroupId_Parent=(SELECT  Min(ParentUserGroupId)   
						FROM         dbo.vlfUserGroupAssignment INNER JOIN
							  dbo.vlfUserGroup ON dbo.vlfUserGroupAssignment.UserGroupId = dbo.vlfUserGroup.UserGroupId
					where  UserId=@userId)
				
			SET @UserGroupId_std=(SELECT Min(UserGroupId) FROM vlfUserGroupAssignment WHERE UserId=@userId) 

			IF ISNULL(@UserGroupId_Parent,99999)<@UserGroupId_std
				set @UserGroupId=@UserGroupId_Parent
			else 
				set @UserGroupId=@UserGroupId_std

			SELECT DISTINCT OrganizationId  
			   ,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName  
			   ,Latitude,Longitude  
			   ,Replace(Description,char(39)+''+char(39),char(39)) AS Description  
			   ,ContactPersonName  
			   ,ContactPhoneNum  
			   ,Radius  
			   ,ISNULL(Email,' ') AS Email  
			   ,ISNULL(Phone,' ') AS Phone  
			   ,ISNULL(TimeZone,0) AS TimeZone  
			   ,ISNULL(DayLightSaving,0) AS DayLightSaving  
			   ,ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving  
			   ,ISNULL(StreetAddress,' ') AS StreetAddress  
			   ,Radius  
			   ,ISNULL([Public], 1) AS [Public]  
			   , LandmarkId AS lid  
			   , Points = 'POLYGON((' + STUFF(
												(  
													SELECT ','+ LTRIM(Str(lp.Longitude, 25, 8)) + ' ' + LTRIM(Str(lp.Latitude, 25, 8)) FROM dbo.vlfLandmarkPointSet AS lp  
													WHERE lp.LandmarkId = x.LandmarkId  
													FOR XML PATH(''), TYPE
												).value('.','nvarchar(max)'), 1, 1, ''
											  ) + 
								  '))'
				,ISNULL(LanCat.DomainMetadataId, 0) AS CategoryId
				,ISNULL(LanCat.MetadataValue, '') AS CategoryName  
		   FROM	
		   (  
			   SELECT * FROM vlfLandmark WHERE organizationid=@organizationId and   ISNULL(CreateUserID,0) <>@userId 
			   EXCEPT  
			   (  
				SELECT * FROM vlfLandmark 
				WHERE organizationid=@organizationId 
				and     ([Public]<>1   and @UserGroupId>2)  
				)  

				UNION

	  			SELECT * FROM vlfLandmark WHERE organizationid=@organizationId   and ISNULL(CreateUserID,0) =@userId 

		   ) x  
		   Left Outer Join 
						(Select	DMM.DomainMetadataId, 
								DM.MetadataValue,
								DMM.RecordId
							From dbo.DomainMetadataMapping DMM (NOLOCK)
								Inner Join dbo.DomainMetadata DM (NOLOCK) ON DMM.DomainMetadataId = DM.DomainMetadataId
							Where DM.DomainId=1 AND DM.OrganizationId=@organizationId 
						) LanCat
				ON x.LandmarkId=LanCat.RecordId
		   Where @categoryId=0 OR LanCat.DomainMetadataId=@categoryId
		  ORDER BY LandmarkName 

		RETURN 
    END		


  
	 -- Assigned Landmark for G4S, others use private/public logic  
	 IF @organizationId = 480 OR @organizationId = 952 OR @organizationId = 999952 OR @organizationId = 999954  
		 BEGIN  
		  DECLARE @IsNccUser INT  
		  IF EXISTS (SELECT * FROM vlfUserGroupAssignment WHERE UserId=@userId AND UserGroupId=36) --36: NCC UserGroup, NCC UserGroup can only see NCC Fleet's Landmark  
		   SET @IsNccUser = 1;  
		  ELSE  
		   SET @IsNccUser = 0;  
	  
			  SELECT DISTINCT OrganizationId  
				   ,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName  
				   ,Latitude,Longitude  
				   ,Replace(Description,char(39)+''+char(39),char(39)) AS Description  
				   ,ContactPersonName  
				   ,ContactPhoneNum  
				   ,Radius  
				   ,ISNULL(Email,' ') AS Email  
				   ,ISNULL(Phone,' ') AS Phone  
				   ,ISNULL(TimeZone,0) AS TimeZone  
				   ,ISNULL(DayLightSaving,0) AS DayLightSaving  
				   ,ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving  
				   ,ISNULL(StreetAddress,' ') AS StreetAddress  
				   ,Radius  
				   ,ISNULL([Public], 1) AS [Public]  
				   , LandmarkId AS lid  
				   , Points = 'POLYGON((' + STUFF(
													(  
														SELECT ','+ LTRIM(Str(lp.Longitude, 25, 8)) + ' ' + LTRIM(Str(lp.Latitude, 25, 8)) FROM dbo.vlfLandmarkPointSet AS lp  
														WHERE lp.LandmarkId = x.LandmarkId  
														FOR XML PATH(''), TYPE
													).value('.','nvarchar(max)'), 1, 1, ''
												  ) + 
									  '))'
					,ISNULL(LanCat.DomainMetadataId, 0) AS CategoryId
					,ISNULL(LanCat.MetadataValue, '') AS CategoryName  
			   FROM  
			   (  
					   SELECT * FROM vlfLandmark WHERE organizationid=@organizationId  
					  
					   EXCEPT  
						 
					   (  
							SELECT l1.* FROM vlfLandmark l1  
								INNER JOIN vlfFleetObjects f ON l1.LandmarkId = f.ObjectId  
							WHERE l1.organizationid=@organizationId AND f.ObjectName = 'landmark' AND --f.FleetId NOT IN (SELECT FleetId FROM vlfFleetUsers WHERE UserId=@userId)  
								( (@IsNccUser = 0 AND f.FleetId NOT IN (SELECT FleetId FROM vlfFleetUsers WHERE UserId=@userId))  
								 OR (@IsNccUser = 1 AND f.FleetId <> 16945)  
								)  
						  
							EXCEPT  
						  
							SELECT l2.* FROM vlfLandmark l2  
								INNER JOIN vlfFleetObjects f ON l2.LandmarkId = f.ObjectId  
							WHERE l2.organizationid=@organizationId AND f.ObjectName = 'landmark' AND --f.FleetId IN (SELECT FleetId FROM vlfFleetUsers WHERE UserId=@userId)  
								( (@IsNccUser = 0 AND  f.FleetId IN (SELECT FleetId FROM vlfFleetUsers WHERE UserId=@userId) )  
								  OR (@IsNccUser = 1 AND f.FleetId = 16945)  
								)  
					   )  
			   ) x  
			   	Left Outer Join 
								(Select	DMM.DomainMetadataId, 
										DM.MetadataValue,
										DMM.RecordId
									From dbo.DomainMetadataMapping DMM (NOLOCK)
										Inner Join dbo.DomainMetadata DM (NOLOCK) ON DMM.DomainMetadataId = DM.DomainMetadataId
									Where DM.DomainId=1 AND DM.OrganizationId=@organizationId 
								) LanCat
						ON x.LandmarkId=LanCat.RecordId
				Where @categoryId=0 OR LanCat.DomainMetadataId=@categoryId
			  ORDER BY LandmarkName  
		END  
	 ELSE  
		BEGIN  
		   DECLARE @v_viewUserSecurityLevel INT  
		   SELECT @v_viewUserSecurityLevel = ISNULL(MIN(ug.SecurityLevel), 999)   
		   FROM vlfUserGroupAssignment uga LEFT JOIN vlfUserGroup ug ON uga.UserGroupId = ug.UserGroupId   
		   WHERE uga.UserId=@userId  
			 
		   SELECT distinct l.OrganizationId  
			   ,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName  
			   ,Latitude,Longitude  
			   ,Replace(Description,char(39)+''+char(39),char(39)) AS Description  
			   ,ContactPersonName  
			   ,ContactPhoneNum  
			   ,Radius  
			   ,ISNULL(Email,' ') AS Email  
			   ,ISNULL(Phone,' ') AS Phone  
			   ,ISNULL(TimeZone,0) AS TimeZone  
			   ,ISNULL(DayLightSaving,0) AS DayLightSaving  
			   ,ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving  
			   ,ISNULL(StreetAddress,' ') AS StreetAddress  
			   ,Radius  
			   ,ISNULL([Public], 1) AS [Public]  
			   , l.LandmarkId AS lid  
			   , Points = 'POLYGON((' + STUFF(
												(  
													SELECT ','+ LTRIM(Str(lp.Longitude, 25, 8)) + ' ' + LTRIM(Str(lp.Latitude, 25, 8)) FROM dbo.vlfLandmarkPointSet AS lp  
													WHERE lp.LandmarkId = l.LandmarkId  
													FOR XML PATH(''), TYPE
												).value('.','nvarchar(max)'), 1, 1, ''
											  ) + 
								  '))'  
				,ISNULL(LanCat.DomainMetadataId, 0) AS CategoryId
				,ISNULL(LanCat.MetadataValue, '') AS CategoryName 
		   FROM vlfLandmark l  
				LEFT JOIN vlfUserGroupAssignment uga on l.CreateUserID = uga.UserID  
				LEFT JOIN vlfUserGroup ug ON uga.UserGroupId = ug.UserGroupId  
				Left Outer Join 
								(Select	DMM.DomainMetadataId, 
										DM.MetadataValue,
										DMM.RecordId
									From dbo.DomainMetadataMapping DMM (NOLOCK)
										Inner Join dbo.DomainMetadata DM (NOLOCK) ON DMM.DomainMetadataId = DM.DomainMetadataId
									Where DM.DomainId=1 AND DM.OrganizationId=@organizationId AND (@categoryId=0 OR DMM.DomainMetadataId=@categoryId)
								) LanCat
						ON l.LandmarkId=LanCat.RecordId 
		   WHERE l.OrganizationId=@organizationId AND (  
				([Public] IS NULL OR [Public]=1)   
				OR CreateUserID = @userId     
				OR (@organizationId=999630 AND (@v_viewUserSecurityLevel=1 OR @v_viewUserSecurityLevel=2))   
				OR (@organizationId <> 999630 AND @v_viewUserSecurityLevel < ISNULL(ug.SecurityLevel, 999))  
				  
			   )  
			   AND (@categoryId=0 OR LanCat.DomainMetadataId=@categoryId)
		  ORDER BY LandmarkName  
		END  
END  



GO


