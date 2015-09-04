
USE [SentinelFM]
GO
/****** Object:  UserDefinedFunction [dbo].[GetEmailsListByServiceConfigId]    Script Date: 7/23/2015 1:49:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:                           <Jacky Jiang>
-- Create date: <2013-12-18>
-- Description:    <Get the info for generating the notification>
-- =============================================
ALTER FUNCTION [dbo].[GetEmailsListByServiceConfigId] 
(
               -- Add the parameters for the function here
               @organizationId int,
               @serviceConfigId int,
               @boxId int,
               @objects nvarchar(100) = null,
               @objectId int = 0,
               @landmarkId int = 0
)
RETURNS 
@TableInfo TABLE 
(
               -- Add the column definitions for the TABLE variable here  
               Emailslist nvarchar(max),
               [Subject] nvarchar(max),
               [Body] nvarchar(max),
               LandmarkName nvarchar(max),
               FleetsName nvarchar(1000),
               Model varchar(200),
               ModelYear varchar(200),
               Make varchar(200),
               Vin varchar(200),
               CustomField1 varchar(200),
               CustomField2 varchar(200),
               CustomField3 varchar(200),
               LandmarkId int,
               LmkContactName varchar(100),
               LmkPhoneNum varchar(20),
			   VehicleOperationalState int
)
AS
BEGIN
               -- Fill the table variable with the rows for your result set
                 DECLARE @vehicleDescription nvarchar(MAX);
      DECLARE @email nvarchar(max);
      DECLARE @allemail nvarchar(max);
                 DECLARE @emailLevel nvarchar(50);
      SET @vehicleDescription = '';
      SET @allemail = '';
                 DECLARE @recepient nvarchar(max);
      DECLARE @subject nvarchar(max);
      DECLARE @messageSubject nvarchar(max);    
      DECLARE @timeZone int;
      DECLARE @landmarkName nvarchar(max);
      DECLARE @landmarkEmail nvarchar(max);            
      DECLARE @messageBody NVARCHAR(max);
      DECLARE @assignedFleetId int;      
                 DECLARE @tmpEmailVal VARCHAR(MAX);
                 DECLARE @model varchar(200);
                 DECLARE @modelYear int;
                 DECLARE @make varchar(200);
                 DECLARE @vin varchar(200);
				 DECLARE @vehicleOperationalState int;
                 DECLARE @customField1 varchar(255);
                 DECLARE @customField2 varchar(255);
                 DECLARE @customField3 varchar(255);
                 DECLARE @fleetsName nvarchar(1000);
                 DECLARE @lmkContactName varchar(100);
                 DECLARE @lmkPhoneNum varchar(20);

                 SET @tmpEmailVal = '';
      SET @recepient = '';      
      SET @assignedFleetId = 0;
                 SET @model = '';
                 SET @modelYear = 0;
                 SET @make = '';
                 SET @vin = '';
				 SET @vehicleOperationalState = 0;
                 SET @customField1 = '';
                 SET @customField2 = '';
                 SET @customField3 = '';
                 SET @fleetsName = '';
                 SET @lmkContactName = '';
                 SET @lmkPhoneNum = '';
               -- Add the SELECT statement with parameter references here
               IF @serviceConfigId > 0
      BEGIN       
            SELECT @messageSubject = [Subject], @messageBody = [Message], @allemail = RecipientsList, @emailLevel = EmailLevel
            FROM dbo.vlfServiceConfigNotification 
            WHERE dbo.vlfServiceConfigNotification.ServiceConfigId = @serviceConfigId;
      END
      IF @emailLevel IS NULL OR @emailLevel = ''
                 BEGIN
                              SET @emailLevel = '';
                 END

                 DECLARE @useFleetNotifyEmail TINYINT;
                 DECLARE @useFleetWarningEmail TINYINT;
                 DECLARE @useFleetCriticalEmail TINYINT;
                 DECLARE @useVehicleEmail TINYINT;
                 DECLARE @useLandmarkEmail TINYINT;
                 DECLARE @useFleetAllEmail TINYINT;

                 SET @useFleetNotifyEmail = 0;
                 SET @useFleetWarningEmail = 0;
                 SET @useFleetCriticalEmail = 0;
                 SET @useVehicleEmail = 0;
                 SET @useLandmarkEmail = 0;
                 SET @useFleetAllEmail = 0;

                 DECLARE @DELIMITER VARCHAR(5);
                 DECLARE @LIST      VARCHAR(MAX);

                 SET @DELIMITER = ';';
                 SET @LIST = @emailLevel;
                 DECLARE @tmpSelect VARCHAR(MAX);
                 DECLARE @LENSTRING INT
                 
                 IF @emailLevel <> ''
                 BEGIN
                              WHILE LEN( @LIST ) > 0 
         BEGIN 
         
            SELECT @LENSTRING = 
               (CASE CHARINDEX( @DELIMITER, @LIST ) 
                   WHEN 0 THEN LEN( @LIST ) 
                   ELSE ( CHARINDEX( @DELIMITER, @LIST ) -1 )
                END
               ) 
                                
           SET @tmpSelect = SUBSTRING(@LIST, 1, @LENSTRING);
           
                                 IF @tmpSelect = 'Vehicle'
                                 BEGIN
                                                            SET @useVehicleEmail = 1;
                                 END     

                                 IF @tmpSelect = 'Landmark'
                                 BEGIN
                                                            SET @useLandmarkEmail = 1;
                                 END

                                 IF @tmpSelect = 'FleetCritical'
                                 BEGIN
                                                            SET @useFleetCriticalEmail = 1;
                                 END

                                 IF @tmpSelect = 'FleetWarning'
                                 BEGIN
                                                            SET @useFleetWarningEmail = 1;
                                 END

                                 IF @tmpSelect = 'FleetNotify'
                                 BEGIN
                                                            SET @useFleetNotifyEmail = 1;
                                 END

                                 IF @tmpSelect = 'FleetAll'
                                 BEGIN
                                                            SET @useFleetAllEmail = 1;
                                 END
                                             
            SELECT @LIST = 
               (CASE ( LEN( @LIST ) - @LENSTRING ) 
                   WHEN 0 THEN '' 
                   ELSE RIGHT( @LIST, LEN( @LIST ) - @LENSTRING - 1 ) 
                END
                                                            )                                                                                                                                                                      
                              END
                 END       
     
                SELECT @tmpEmailVal = vlfVehicleInfo.Email, @model = vlfModel.ModelName, @modelYear = vlfVehicleInfo.ModelYear, @make = vlfMake.MakeName, @customField1 = Field1, @customField2 = Field2, @customField3 = Field3, @vin = VinNum, @vehicleOperationalState = ISNULL(vlfVehicleInfo.OperationalState, 100) FROM vlfVehicleInfo 
                              INNER JOIN vlfMakeModel ON vlfMakeModel.MakeModelId = vlfVehicleInfo.MakeModelId
                              INNER JOIN vlfMake ON vlfMake.MakeId = vlfMakeModel.MakeId
                              INNER JOIN vlfModel ON vlfModel.ModelId = vlfMakeModel.ModelId
                              INNER JOIN vlfVehicleAssignment ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId 
                              WHERE vlfVehicleAssignment.BoxId = @boxId;
                 
                 IF @useVehicleEmail = 1
                 BEGIN
                              
                              IF @tmpEmailVal IS NOT NULL AND @tmpEmailVal <> ''
                              BEGIN
                                             IF @allemail IS NOT NULL AND @allemail <> ''
                                             BEGIN
                                                            SET @allemail = @allemail + ';' + @tmpEmailVal;
                                             END
                                             ELSE
                                             BEGIN
                                                            SET @allemail = @tmpEmailVal;
                                             END
                              END
                 END
                 DECLARE @fName nvarchar(200);
                 IF @organizationId = 952
                 BEGIN
                 DECLARE vendor_cursor CURSOR FOR
                  SELECT fe.Email, fe.FleetId, f.FleetName FROM vlfVehicleAssignment 
                        INNER JOIN vlfFleetVehicles fv ON vlfVehicleAssignment.VehicleId = fv.VehicleId
                        inner JOIN vlfFleetEmails fe ON fe.FleetId = fv.FleetId     
                                                                                          INNER JOIN vlfFleet f ON f.FleetId = fv.FleetId         
                        where vlfVehicleAssignment.BoxId = @boxId AND fe.Landmark = 1;
                  OPEN vendor_cursor
                  FETCH NEXT FROM vendor_cursor into @email, @assignedFleetId, @fName

                  WHILE @@fetch_status = 0
                  BEGIN
                                                                                          IF CHARINDEX(@fName, @fleetsName) < 1
                                                                                          BEGIN
                                                                                                         IF @fleetsName = '' OR @fleetsName = NULL
                                                                                                         BEGIN
                                                                                                                        SET @fleetsName = @fName;
                                                                                                         END
                                                                                                         ELSE
                                                                                                         BEGIN
                                                                                                                        SET @fleetsName = @fleetsName + ', ' + @fName;
                                                                                                         END
                                                                                          END

                        IF @objects = 'Fleet' AND @objectId > 0
                        BEGIN
                              IF CAST(@assignedFleetId AS INT) <> @objectId
                              BEGIN
                                    FETCH NEXT FROM vendor_cursor into @email, @assignedFleetId, @fName;
                                    CONTINUE;
                              END
                        END
                        IF @allemail IS NULL OR @allemail = ''
                        BEGIN
                              SET @allemail = @email;       
                        END
                        ELSE
                        BEGIN
                              SET @allemail = @allemail + ';' + @email;
                        END
            
                        FETCH NEXT FROM vendor_cursor into @email, @assignedFleetId, @fName;
                  END
                  CLOSE vendor_cursor;
                  DEALLOCATE vendor_cursor;
            END
            ELSE
            BEGIN
                                                                           

                                                              DECLARE @t table (objectId int)
                                                              INSERT INTO @t (objectId) 
                                                              SELECT ObjectID FROM vlfServiceAssignments WHERE ServiceConfigID=@serviceConfigId AND objects='Fleet' AND Deleted=0

                                                              DECLARE @critical int;
                                                              DECLARE @warning int;
                                                             DECLARE @notify int;
                                                              
                  DECLARE vendor_cursor CURSOR FOR

                  SELECT fe.Email, fe.FleetId, fe.Critical, fe.Warning, fe.Notify, f.FleetName FROM vlfVehicleAssignment 
                        INNER JOIN vlfFleetVehicles fv ON vlfVehicleAssignment.VehicleId = fv.VehicleId
                        inner JOIN vlfFleetEmails fe ON fe.FleetId = fv.FleetId
                                                                                          INNER JOIN vlfFleet f ON f.FleetId = fv.FleetId              
                        where vlfVehicleAssignment.BoxId = @boxId;
                  OPEN vendor_cursor
                  FETCH NEXT FROM vendor_cursor into @email, @assignedFleetId, @critical, @warning, @notify, @fName;

                  WHILE @@fetch_status = 0
                  BEGIN
                                                                                          IF CHARINDEX(@fName, @fleetsName) < 1
                                                                                          BEGIN
                                                                                                         IF @fleetsName = '' OR @fleetsName = NULL
                                                                                                         BEGIN
                                                                                                                        SET @fleetsName = @fName;
                                                                                                         END
                                                                                                         ELSE
                                                                                                         BEGIN
                                                                                                                        SET @fleetsName = @fleetsName + ', ' + @fName;
                                                                                                         END
                                                                                          END
                        IF @objects = 'Fleet' AND @objectId > 0
                        BEGIN
                              --IF CAST(@assignedFleetId AS INT) <> @objectId
                                                                                                           IF (NOT EXISTS(SELECT * FROM @t WHERE objectId=@assignedFleetId))
                              BEGIN
                                    FETCH NEXT FROM vendor_cursor into @email, @assignedFleetId, @critical, @warning, @notify, @fName;
                                    CONTINUE;
                              END
                        END

                                                                                          SET @tmpEmailVal = '';
                                                                                          IF @useFleetAllEmail = 1
                                                                                          BEGIN
                                                                                                         SET @tmpEmailVal = @email;
                                                                                          END
                                                                                          ELSE
                                                                                          BEGIN
                                                                                                         IF @useFleetCriticalEmail = 1 AND @critical = 1
                                                                                                         BEGIN
                                                                                                                        SET @tmpEmailVal = @email;
                                                                                                         END
                                                                                                         ELSE IF @useFleetWarningEmail = 1 AND @warning = 1
                                                                                                         BEGIN
                                                                                                                        SET @tmpEmailVal = @email; 
                                                                                                         END
                                                                                                         ELSE IF @useFleetNotifyEmail = 1 AND @notify = 1
                                                                                                         BEGIN
                                                                                                                        SET @tmpEmailVal = @email; 
                                                                                                         END
                                                                                          END
                                                                                          

                                                                                          IF @tmpEmailVal <> ''
                                                                                          BEGIN
                                                                                                         IF @allemail IS NULL OR @allemail = ''
                                                                                                         BEGIN
                                                                                                                          SET @allemail = @tmpEmailVal;       
                                                                                                         END
                                                                                                         ELSE
                                                                                                         BEGIN
                                                                                                                          SET @allemail = @allemail + ';' + @tmpEmailVal;
                                                                                                         END
                                                                                          END
                                             
                        FETCH NEXT FROM vendor_cursor into @email, @assignedFleetId, @critical, @warning, @notify, @fName;
                  END
                  CLOSE vendor_cursor;
                  DEALLOCATE vendor_cursor;


     END

      SET @landmarkName = 'on road landmark';
      SET @landmarkEmail = '';
                 SET @tmpEmailVal = '';
      IF @landmarkId > 0
      BEGIN   
                                             DECLARE @landmarkOrganizationId int;
                                             SET @landmarkOrganizationId = 0;
                                             SELECT TOP 1 @landmarkName = LandmarkName, @tmpEmailVal = Email, @landmarkOrganizationId = OrganizationId, @lmkContactName = ContactPersonName, @lmkPhoneNum = ContactPhoneNum FROM SentinelFM.dbo.vlfLandmark WHERE LandmarkId=@landmarkId;  

                                             IF @landmarkOrganizationId <> @organizationId
                                             BEGIN
                                                            SET @landmarkName = 'Property-' + CAST(@landmarkId AS nvarchar);
                                                            SET @landmarkId = 0 - @landmarkId;
                                             END
                                             ELSE 
                                             BEGIN
                                                            IF @tmpEmailVal IS NOT NULL AND @tmpEmailVal <> '' AND @useLandmarkEmail = 1
                                                            BEGIN
                                                                           IF @allemail IS NOT NULL AND @allemail <> ''
                                                                           BEGIN
                                                                                          SET @allemail = @allemail + ';' + @tmpEmailVal;
                                                                           END
                                                                           ELSE
                                                                           BEGIN
                                                                                          SET @allemail = @tmpEmailVal;
                                                                           END
                                                            END    
                                             END                                                                                                                                   
                              END

                 INSERT INTO @TableInfo VALUES(@allemail, @messageSubject, @messageBody, @landmarkName, @fleetsName, @model, @modelYear, @make, @vin, @customField1, @customField2, @customField3, @landmarkId, @lmkContactName, @lmkPhoneNum, @vehicleOperationalState);

               RETURN 
END


