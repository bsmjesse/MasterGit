
USE [SentinelFM]
GO
/****** Object:  StoredProcedure [dbo].[sp_vlfLandmarkNotification]    Script Date: 7/23/2015 1:35:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:        Jacky  
-- Create date: 2012-11-14  
-- Description:   Create a store procedure to proccess the evtNotification and evtEvents table  
-- =============================================  
ALTER PROCEDURE [dbo].[sp_vlfLandmarkNotification]   
      -- Add the parameters for the stored procedure here  
      @ruleName nvarchar(30),  
      @serviceConfigId int,  
      @organizationId int,  
      @boxId int,  
      @landmarkId int,  
      @date DateTime,     
      @fleetId int = 0,         
      @eventId int = 0,  
   @serviceId int = 0,  
      @landmarkinEventId bigint = 0,  
      @serviceName nvarchar(50) = null,  
      @userName nvarchar(100) = null,  
      @speed int = 0,  
      @speedThreshold nvarchar(50) = null,  
      @durationThreshold nvarchar(50) = null,  
      @address nvarchar(200) = null,  
      @latitude decimal(9,6) = 0,  
      @longitude decimal(9,6) = 0,           
      @driverId int = 0,  
      @vehicleId int = 0,  
      @metric int = 1,  
      @speedZoneType int = 1,  
      @roadSpeed int = 0,  
      @eventDuration int = 0,  
      @objects nvarchar(100) = '',  
      @objectId int = 0,  
      @eventTime DateTime = null,  
   @metricUnit nvarchar(100) = 'km',  
   @violationId int = 0,  
   @cltValue nvarchar(200) = '',  
   @eopValue nvarchar(200) = '',  
   @eotValue nvarchar(200) = '',  
   @staValue nvarchar(200) = '',  
   @flipValue nvarchar(200) = '',  
   @flisvalue nvarchar(200) = '',  
   @rpmValue nvarchar(200) = '',  
   @tvdValue nvarchar(200) = '',  
   @dtcValue nvarchar(max) = '',  
   @lsd nvarchar(200) = '',  
   @lsdSetting nvarchar(300) = '',  
   @scheduleSend DateTime = '',  
   @personId nvarchar(100) = '',  
   @mainbattery nvarchar(100) = '',  
   @customProp nvarchar(1024) = '',
   @serviceDuration int = 0
AS  
BEGIN   
      -- SET NOCOUNT ON added to prevent extra result sets from  
      -- interfering with SELECT statements.  
      SET NOCOUNT ON;     
   DECLARE @sendStatus int;  
   SET @sendStatus = 0;  
      --Dirty and quick fix up serviceconfigid=436  
    
         IF @organizationId = 951 AND @serviceConfigId = 436  
         BEGIN  
              IF NOT EXISTS(SELECT BoxMsgInTypeId FROM [192.168.9.41].[SentinelFM].[dbo].[vlfMsgInHst] WITH (NOLOCK) WHERE boxid=@boxId AND origindatetime=@date AND BoxMsgInTypeId=3)  
              BEGIN  
                     --SELECT 1;  
                     --RETURN;  
      SET @sendStatus = 2;  
              END  
         END  
  
         --dirty and quick fix ends up  
  
      DECLARE @ruleValue nvarchar(50);  
      SET @ruleValue = @durationThreshold;  
    -- Insert statements for procedure here  
  
  
   IF @landmarkinEventId > 0  
   BEGIN  
   If @eventId = 20   
   BEGIN  
    UPDATE dbo.evtEvents SET IsEvent = 1 WHERE ID = @landmarkinEventId;             
   END  
      --ELSE If (@ruleName = 'Speed' OR @ruleName = 'RoadSpeed') AND @landmarkinEventId > 0  
   ELSE IF @eventId = 21  
   BEGIN  
    UPDATE dbo.evtViolations SET IsEvent = 1, Duration = @eventDuration WHERE ID = @landmarkinEventId;  
    SET @ruleValue = @speedThreshold;  
   END     
   END  
       
        
      DECLARE @vehicleDescription nvarchar(MAX);  
      DECLARE @email nvarchar(max);  
      DECLARE @allemail nvarchar(max);  
   DECLARE @emailLevel nvarchar(50);  
      SET @vehicleDescription = '';  
      SET @allemail = '';  
      IF @boxId > 0  
        
      BEGIN  
            SELECT @vehicleDescription = dbo.vlfVehicleInfo.Description FROM dbo.vlfVehicleAssignment INNER JOIN dbo.vlfVehicleInfo ON dbo.vlfVehicleAssignment.VehicleId = dbo.vlfVehicleInfo.VehicleId WHERE dbo.vlfVehicleAssignment.BoxId = @boxId;        
              
      END     
  
      DECLARE @recepient nvarchar(max);  
      DECLARE @subject nvarchar(max);  
      DECLARE @messageSubject nvarchar(max);      
      DECLARE @timeZone int;  
      DECLARE @landmarkName nvarchar(max);  
      DECLARE @landmarkEmail nvarchar(max);        
      DECLARE @speedUnit NVARCHAR(max);  
      DECLARE @messageBody NVARCHAR(max);        DECLARE @assignedFleetId int;  
      DECLARE @utcTime DateTime;  
   DECLARE @tmpEmailVal VARCHAR(MAX);  
   DECLARE @model varchar(200);  
   DECLARE @modelYear int;  
   DECLARE @make varchar(200);  
   DECLARE @vin varchar(200);  
   DECLARE @customField1 varchar(255);  
   DECLARE @customField2 varchar(255);  
   DECLARE @customField3 varchar(255);  
   DECLARE @fleetsName nvarchar(1000);  
   DECLARE @LmkContactName varchar(100);  
   DECLARE @LmkPhoneNum varchar(20);  
   DECLARE @ProblemDescription nvarchar(255);  
   DECLARE @ServiceContact nvarchar(255);
   DECLARE @assignedFleetName varchar(100);
   DECLARE @formattedDateTime varchar(100);
   DECLARE @msgDuration varchar(100);
   DECLARE @vehicleOperationalState int;
   DECLARE @vehicleOperationalStateName nvarchar(60);
   
   SET @tmpEmailVal = '';  
      SET @recepient = '';  
      SET @speedUnit = '';  
      SET @assignedFleetId = 0;  
   SET @model = '';  
   SET @modelYear = 0;  
   SET @make = '';  
   SET @vin = '';  
   SET @customField1 = '';  
   SET @customField2 = '';  
   SET @customField3 = '';  
   SET @fleetsName = '';  
   SET @LmkContactName = '';  
   SET @LmkPhoneNum = '';  
   SET @ProblemDescription = '';
   SET @ServiceContact = ''; 
   SET @vehicleOperationalState = 0;
   SET @vehicleOperationalStateName = '';
   
   --------------------------------------------  
   SELECT @allemail = Emailslist, @messageSubject = [Subject], @messageBody = [Body],   
   @landmarkName = LandmarkName, @fleetsName = FleetsName, @model = Model, @modelYear = ModelYear, @make = Make, @vin = Vin,
   @customField1 = CustomField1, @customField2 = CustomField2, @customField3 = CustomField3, @landmarkId = LandmarkId, @LmkContactName = LmkContactName, @LmkPhoneNum = LmkPhoneNum, @vehicleOperationalState = VehicleOperationalState
   FROM [dbo].[GetEmailsListByServiceConfigId] (@organizationId, @serviceConfigId, @boxId, @objects, @objectId, @landmarkId);  
   
   IF @vehicleOperationalState = 100
		BEGIN
			SET @vehicleOperationalStateName = 'Available';
		END
   
   IF @vehicleOperationalState = 200
		BEGIN
			SET @vehicleOperationalStateName = 'Unavailable';
		END
   --------------------------------------------    
              
  
      SET @timeZone = 0;  
        
      IF @boxId > 0  
      BEGIN  
            --SELECT @timeZone = vlfStateProvince.TimeZone, @daylightSaving = vlfStateProvince.DayLightSaving FROM vlfVehicleInfo  
            --INNER JOIN vlfStateProvince ON vlfStateProvince.StateProvince = vlfVehicleInfo.StateProvince  
            --WHERE vlfVehicleInfo.VehicleId = @vehicleId;  
                                             SELECT @timeZone = tz from dbo.udf_FindTimeZoneCurrent(@boxId);  
            SET @utcTime = @date;    
                                             
                                             BEGIN TRY
                                                            --Detect all speed service, if it is invalid packet, then discard the message and delete from evtViolation
                                                            DECLARE @tmpServiceId int;
                                                            SELECT @tmpServiceId = ServiceID FROM vlfServiceConfigurations WHERE ServiceConfigID = @serviceConfigId;
                                                            IF @tmpServiceId = 2
                                                            BEGIN
                                                                           DECLARE @tmpValidGPS int;
                                                                           SELECT @tmpValidGPS = ValidGps FROM [192.168.9.41].[SentinelFM].[dbo].[vlfMsgInHst] WHERE BoxId=@boxId AND OriginDateTime=@utcTime;
                                                                           IF @tmpValidGPS > 0
                                                                           BEGIN
                                                                                          SET @sendStatus = 1;
                                                                                          DELETE FROM evtViolations WHERE ID=@violationId;
                                                                           END
                                                            END
                                             END TRY
                                             BEGIN CATCH
                                                            SET @sendStatus = 0;
                                             END CATCH       
  
            SET @date = DATEADD(hh, @timeZone, @date); 
                   SELECT @formattedDatetime = STUFF(REPLACE('/' + CONVERT(CHAR(10), @date, 101),'/0','/'),1,1,'');
            SET @formattedDatetime = @formattedDatetime  + ' ' + CONVERT(VARCHAR(8), @date, 108) + ' ' + right(CONVERT(varchar,@date,100),2);
            IF @eventTime IS NOT NULL AND @eventTime <> ''  
            BEGIN               
                  SET @eventTime = DATEADD(hh, @timeZone, @eventTime);  
            END  
  
  
            SELECT @driverId = DriverId from vlfDriverAssignment WHERE VehicleId=@vehicleId;  
      END  
  
   If @metricUnit = 'miles'  
   BEGIN  
  SET @metricUnit = 'mph';   
   END     
   ELSE  
   BEGIN  
  SET @metricUnit = 'km/h';   
   END  
  
      DECLARE @message nvarchar(max);  
      SET @message = '';  
  
      DECLARE @tmpDriverName nvarchar(max);       
      DECLARE @driverName NVARCHAR(MAX);  
      DECLARE @superName nvarchar(max);  
      DECLARE @lp nvarchar(max);  
   DECLARE @tmpDisputeRoadSpeed nvarchar(50);  
   SET @tmpDisputeRoadSpeed = '';  
      --create message subject  
   DECLARE @errorMessage nvarchar(max);  
   SET @errorMessage = '';  
   BEGIN TRY  
      IF @messageSubject IS NOT NULL AND @messageSubject <> ''  
      BEGIN  
      SET @message = @messageSubject  
        
      if CHARINDEX('[RULE_NAME]',@message) > 0  
      BEGIN  
         SET @message = replace(@message, '[RULE_NAME]', @ruleName);  
      END  
      if CHARINDEX('[USER_NAME]',@message) > 0  
      BEGIN  
         IF @userName IS NOT NULL  
         SET @message = replace(@message, '[USER_NAME]', @userName);  
      END  
  
      if CHARINDEX('[BOX_ID]',@message) > 0  
      BEGIN  
         IF @boxId <> 0  
         SET @message = replace(@message, '[BOX_ID]', CAST(@boxId AS NVARCHAR));  
      END  
  
      if CHARINDEX('[ServiceName]',@message) > 0  
      BEGIN  
         IF @serviceName IS NOT NULL     
         SET @message = replace(@message, '[ServiceName]', @serviceName);            
      END  
  
      if CHARINDEX('[SIGNAL_SPEED]',@message) > 0  
      BEGIN  
      IF @speed  > 0  
         --IF @metric = 2  
         --BEGIN                     
         --      SET @speed = CONVERT(int, @speed / 1.609344);  
         --      SET @metricUnit = 'miles';  
         --END  
         SET  @speedUnit =  CAST(@speed AS NVARCHAR) + ' ' + @metricUnit;  
         SET @message = replace(@message, '[SIGNAL_SPEED]', @speedUnit);              
      END  
  
      if CHARINDEX('[ROAD_SPEED]',@message) > 0  
      BEGIN  
      IF @roadSpeed  > 0  
         --IF @metric = 2  
         --BEGIN                     
         --      IF @speedZoneType < 2  
         --      BEGIN  
         --            SET @roadSpeed = CONVERT(int, @roadSpeed / 1.609344);  
         --      END  
         --      SET @metricUnit = 'miles';  
         --END  
                    
         SET  @speedUnit =  CAST(@roadSpeed AS NVARCHAR) + ' ' + @metricUnit;  
         SET @message = replace(@message, '[ROAD_SPEED]', @speedUnit);              
      END  
  
      if CHARINDEX('[EVENT_NAME]',@message) > 0  
      BEGIN  
         IF @serviceName IS NOT NULL     
         SET @message = replace(@message, '[EVENT_NAME]', @serviceName);                        
      END  
  
      IF @vehicleDescription IS NOT NULL  
      BEGIN   
         if CHARINDEX('[VehicleDescription]',@message) > 0  
         BEGIN  
         SET @message = replace(@message, '[VehicleDescription]', @vehicleDescription);           
         END  
      END  
      ELSE  
      BEGIN  
         SET @vehicleDescription = '(BoxId:)' + CAST(@boxId AS NVARCHAR);  
      END  
              
      if CHARINDEX('[DRIVER_ID]',@message) > 0  
      BEGIN  
         IF @driverId <> 0  
         SET @message = replace(@message, '[DRIVER_ID]', CAST(@driverId AS NVARCHAR));         
      END  
  
      if CHARINDEX('[DRIVER_NAME]',@message) > 0  
      BEGIN  
         IF @organizationId = 123  
         BEGIN         
         SET @tmpDriverName = '';              
         SELECT @tmpDriverName=Field3 FROM vlfVehicleInfo WHERE VehicleId =@vehicleId;  
         IF @tmpDriverName IS NOT NULL  
         BEGIN         
            SET @message = replace(@message, '[DRIVER_NAME]', CAST(@tmpDriverName AS NVARCHAR));     
         END     
         END   
         ELSE  
         BEGIN  
         IF @driverId <> 0   
         BEGIN                                             
            SELECT @driverName = FirstName + ' ' + LastName FROM vlfDriver WHERE DriverId=@driverId;             
            IF @driverName IS NOT NULL  
            BEGIN         
            SET @message = replace(@message, '[DRIVER_NAME]', CAST(@driverName AS NVARCHAR));        
            END     
         END  
         END  
                    
      END  
  
      if CHARINDEX('[LONGITUDE]',@message) > 0  
      BEGIN  
         IF @longitude <> 0  
         SET @message = replace(@message, '[LONGITUDE]',  CAST(@longitude AS NVARCHAR));        
      END  
  
      if CHARINDEX('[LATITUDE]',@message) > 0  
      BEGIN  
      IF @latitude <> 0   
         SET @message = replace(@message, '[LATITUDE]', CAST(@latitude AS NVARCHAR));   
      END  
  
      if CHARINDEX('[ADDRESS]',@message) > 0  
      BEGIN  
      IF @address IS NOT NULL               
         SET @message = replace(@message, '[ADDRESS]', @address);              
      END  
  
      if CHARINDEX('[StDate]',@message) > 0  
      BEGIN  
      IF @date IS NOT NULL  
         SET @message = replace(@message, '[StDate]', CAST(@date AS NVARCHAR) + '(GMT' + CONVERT(nvarchar, @timeZone) + ')');  
      END  
  
      if CHARINDEX('[EVENT_TIME]',@message) > 0  
      BEGIN  
         IF @eventTime IS NOT NULL       
         SET @message = replace(@message, '[EVENT_TIME]', CAST(@eventTime AS nvarchar) + '(GMT' + CONVERT(nvarchar, @timeZone) + ')');  
         ELSE  
         SET @message = replace(@message, '[EVENT_TIME]', CAST(GETDATE() AS NVARCHAR));  
      END  
  
      if CHARINDEX('[LANDMARK_NAME]',@message) > 0  
      BEGIN  
         IF @landmarkName IS NOT NULL  
         SET @message = replace(@message, '[LANDMARK_NAME]', @landmarkName);  
      END  
                
      if CHARINDEX('[LANDMARK_ID]',@message) > 0     
      BEGIN           
       IF @landmarkid <> 0         BEGIN  
          SET @message = replace(@message, '[LANDMARK_ID]', CAST(@landmarkid AS NVARCHAR));       
       END        
      END  
  
      if CHARINDEX('[FLEET_ID]',@message) > 0  
      BEGIN                    
         IF @objects = 'Fleet' AND @objectId > 0  
         BEGIN  
         SET @message = replace(@message, '[FLEET_ID]', CAST(@objectId AS NVARCHAR));         
         END              
      END     
  
      if CHARINDEX('[GOOGLE_LINK]',@message) > 0  
      begin  
         SET @message = replace(@message, '[GOOGLE_LINK]', 'http://maps.google.com/maps?q=' + CAST(@latitude AS NVARCHAR) + ',' + CAST(@longitude AS NVARCHAR));  
      end  
  
      IF CHARINDEX('[SUPERVISOR]',@message) > 0  
      BEGIN  
         IF @vehicleId > 0  
         BEGIN  
         SET @superName = '';  
  
         SELECT @superName = Field4 FROM vlfVehicleInfo WHERE VehicleId = @vehicleId;  
         IF @superName <> '' AND @superName IS NOT NULL  
         BEGIN  
            SET @message = replace(@message, '[SUPERVISOR]', @superName);  
         END  
         END  
      END  
  
      IF CHARINDEX('[LICENSE_PLATE]',@message) > 0  
      BEGIN  
         IF @vehicleId > 0  
         BEGIN  
         SET @lp = '';  
  
         SELECT @lp = LicensePlate FROM vlfVehicleAssignment WHERE VehicleId = @vehicleId;  
         IF @lp <> '' AND @lp IS NOT NULL  
         BEGIN  
            SET @message = replace(@message, '[LICENSE_PLATE]', @lp);  
         END  
         END  
      END  
  
      if CHARINDEX('[DISPUTE_LINK]',@message) > 0  
      begin  
      
        IF @speed  > 0  
        BEGIN  
        SET  @speedUnit =  CAST(@speed AS NVARCHAR) + ' ' + @metricUnit;              
        END  
  
        IF @roadSpeed  > 0  
        BEGIN  
        SET  @tmpDisputeRoadSpeed =  CAST(@roadSpeed AS NVARCHAR) + ' ' + @metricUnit;                          
       END  
                  
       SET @message = replace(@message, '[DISPUTE_LINK]', 'http://preprod.sentinelfm.com/community/dispute.aspx?infractionid=' + CAST(@violationId AS NVARCHAR) + '&q=' + CAST(@latitude AS NVARCHAR) + ',' + CAST(@longitude AS NVARCHAR) + '&errorspeed=' + @tmpDisputeRoadSpeed + '&yourspeed=' + @speedUnit + '&address=' + @address + '&objects=' + @objects + '&objectid=' + CAST(@objectId AS nvarchar) + '&nid=[NOTIFICATION_ID]');  
      end  
  
      if CHARINDEX('[CLT_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CLT_VALUE]', @cltValue);  
      END  
  
      if CHARINDEX('[EOP_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[EOP_VALUE]', @eopValue);  
      END  
  
      if CHARINDEX('[EOT_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[EOT_VALUE]', @eotValue);  
      END  
  
      if CHARINDEX('[STA_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[STA_VALUE]', @staValue);  
      END  
  
      if CHARINDEX('[FLIP_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[FLIP_VALUE]', @flipValue);  
      END  
  
      if CHARINDEX('[FLIS_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[FLIS_VALUE]', @flisValue);  
      END  
  
      if CHARINDEX('[RPM_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[RPM_VALUE]', @rpmValue);  
      END  
  
      if CHARINDEX('[TVD_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[TVD_VALUE]', @tvdValue);  
      END  
  
      if CHARINDEX('[DTC_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[DTC_VALUE]', @dtcValue);  
      END  
  
      if CHARINDEX('[VEHICLE_YEAR]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_YEAR]', @modelYear);  
      END  
  
      if CHARINDEX('[VEHICLE_MODEL]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_MODEL]', @model);  
      END     
  
      if CHARINDEX('[VEHICLE_MAKE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_MAKE]', @make);  
      END  
  
      if CHARINDEX('[VEHICLE_VIN]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_VIN]', @vin);  
      END  
	  
	  if CHARINDEX('[VEHICLE_OPERATIONAL_STATE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_OPERATIONAL_STATE]', @vehicleOperationalStateName);  
      END 
  
      if CHARINDEX('[CUSTOM_FIELD1]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CUSTOM_FIELD1]', @customField1);  
      END  
  
      if CHARINDEX('[CUSTOM_FIELD2]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CUSTOM_FIELD2]', @customField2);  
      END  
  
      if CHARINDEX('[CUSTOM_FIELD3]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CUSTOM_FIELD3]', @customField3);  
      END  
  
      if CHARINDEX('[FLEETS_NAME]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[FLEETS_NAME]', @fleetsName);  
      END  
  
      if CHARINDEX('[LSD]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[LSD]', @lsdSetting);  
      END  
  
      if CHARINDEX('[LSD_RULESETTINGS]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[LSD_RULESETTINGS]', @lsdSetting);  
      END  
      if CHARINDEX('[KEYFOB_ID]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[KEYFOB_ID]', @personId);  
      END  
     
      IF CHARINDEX('[CONTACT_NAME]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CONTACT_NAME]', @LmkContactName); --??  
      END  
     
      IF CHARINDEX('[CONTACT_PHONE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CONTACT_PHONE]', @LmkPhoneNum); --??  
      END  
     
      IF CHARINDEX('[PROBLEM_DESCRIPTION]',@message) > 0     
      BEGIN   
       SELECT @ProblemDescription = ISNULL(Field1,'') FROM vlfVehicleExtraServiceHistory WHERE VehicleId=@vehicleId;  
       SET @message = REPLACE(@message, '[PROBLEM_DESCRIPTION]', @ProblemDescription); --??  
      END 
      
      IF CHARINDEX('[SERVICE_CONTACT]',@message) > 0     
                 BEGIN  
                                SELECT @ServiceContact = ISNULL(Field2,'') FROM vlfVehicleExtraServiceHistory  WHERE VehicleId=@VehicleId; 
                                SET @message = REPLACE(@message, '[SERVICE_CONTACT]', @ServiceContact); --?? 
                 END 
  
      IF CHARINDEX('[MAIN_BATTERY]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[MAIN_BATTERY]', @mainbattery); --??  
      END  
  
      IF CHARINDEX('[S4]',@message) > 0     
      BEGIN        
       SET @message = REPLACE(@message, '[S4]', dbo.GetStringValueFromCustomProperties(@customProp, 'S4'));  
      END  

               IF CHARINDEX('[ASSIGNED_FLEET]',@message) > 0     
               BEGIN        
                              IF @objects = 'Fleet' AND @objectId > 0
                              BEGIN
                                             select @assignedFleetName = Nodecode from  dbo.udf_VehicleCostCenters (@organizationId) where VehicleID=@vehicleId;
                                             IF @assignedFleetName IS NOT NULL
                                             BEGIN
                                                            SET @message = REPLACE(@message, '[ASSIGNED_FLEET]', @assignedFleetName);
                                             END
                              END
      
               END  

   IF CHARINDEX('[FORMATTED_DATETIME]',@message) > 0     
   BEGIN        
    SET @message = REPLACE(@message, '[FORMATTED_DATETIME]', @formattedDateTime);  
   END
   
   IF CHARINDEX('[SERVICE_DURATION]',@message) > 0     
   BEGIN          
                                                
               SET @msgDuration = dbo.GetStringValueFromCustomProperties(@customProp, 'DURATION');
	IF @msgDuration IS NOT NULL AND @msgDuration <> ''
	BEGIN
               SET @serviceDuration = CAST(@msgDuration AS int);
               SET @msgDuration = CONVERT(varchar, DATEADD(ss, @serviceDuration, 0), 108);
    SET @message = REPLACE(@message, '[SERVICE_DURATION]', @msgDuration);  
   END
   END
      
      END     
              
  
      IF @message IS NULL OR @message = ''  
      BEGIN  
      SET @subject = @serviceName + '(' + CAST(@speed AS NVARCHAR) + ')' + ' occurred for vehicle:' + @vehicleDescription+ ' in landmark(' + @landmarkName +  ') at ' + CAST(@date AS NVARCHAR);           
      END  
        
      SET @subject = @message;  
  
      IF @subject IS NULL OR @subject = ''  
      BEGIN  
      SET @subject = 'Alert';  
      END  
        
        
      --Create message body  
      IF @messageBody IS NOT NULL AND @messageBody <> ''  
      BEGIN  
      SET @message = @messageBody  
              
      if CHARINDEX('[RULE_NAME]',@message) > 0  
      BEGIN  
         SET @message = replace(@message, '[RULE_NAME]', @ruleName);  
         SET @errorMessage =  @errorMessage + '1' + @message;  
      END  
      if CHARINDEX('[USER_NAME]',@message) > 0  
      BEGIN  
         IF @userName IS NOT NULL  
         SET @message = replace(@message, '[USER_NAME]', @userName);  
       SET @errorMessage =  @errorMessage  + '2' + @message;  
      END  
  
      if CHARINDEX('[BOX_ID]',@message) > 0  
      BEGIN  
         IF @boxId <> 0  
         SET @message = replace(@message, '[BOX_ID]', CAST(@boxId AS NVARCHAR));  
  
       SET @errorMessage =  @errorMessage  + '3' + @message;  
      END  
  
      if CHARINDEX('[ServiceName]',@message) > 0  
      BEGIN  
         IF @serviceName IS NOT NULL     
         SET @message = replace(@message, '[ServiceName]', @serviceName);            
       SET @errorMessage =  @errorMessage  + '4' + @message;  
      END  
  
      if CHARINDEX('[SIGNAL_SPEED]',@message) > 0  
      BEGIN  
      IF @speed  > 0           
         SET  @speedUnit =  CAST(@speed AS NVARCHAR) + ' ' + @metricUnit;  
         SET @message = replace(@message, '[SIGNAL_SPEED]', @speedUnit);    
         SET @errorMessage =  @errorMessage  + '5' + @message;            
      END  
  
      if CHARINDEX('[ROAD_SPEED]',@message) > 0  
      BEGIN  
      IF @roadSpeed  > 0                            
         SET  @speedUnit =  CAST(@roadSpeed AS NVARCHAR) + ' ' + @metricUnit;  
         SET @message = replace(@message, '[ROAD_SPEED]', @speedUnit);   
         SET @errorMessage =  @errorMessage  + '6' + @message;             
      END  
  
      if CHARINDEX('[EVENT_NAME]',@message) > 0  
      BEGIN  
         IF @serviceName IS NOT NULL     
         SET @message = replace(@message, '[EVENT_NAME]', @serviceName);    
       SET @errorMessage =  @errorMessage  + '7' + @message;                      
      END  
  
      IF @vehicleDescription IS NOT NULL  
      BEGIN   
         if CHARINDEX('[VehicleDescription]',@message) > 0  
         BEGIN  
                                              IF @vehicleDescription IS NOT NULL
                                             BEGIN
                                                            SET @message = replace(@message, '[VehicleDescription]', @vehicleDescription);           
                                                            SET @errorMessage =  @errorMessage  + '8' + @message; 
                                              END          
         END  
      END  
      ELSE  
      BEGIN  
         SET @vehicleDescription = '(BoxId:)' + CAST(@boxId AS NVARCHAR);  
      END  
              
      if CHARINDEX('[DRIVER_ID]',@message) > 0  
      BEGIN  
         IF @driverId <> 0  
         SET @message = replace(@message, '[DRIVER_ID]', CAST(@driverId AS NVARCHAR));   
       SET @errorMessage =  @errorMessage  + '9' + @message;        
      END  
  
      if CHARINDEX('[DRIVER_NAME]',@message) > 0  
      BEGIN  
         IF @organizationId = 123  
         BEGIN         
         SET @tmpDriverName = '';              
         SELECT @tmpDriverName=Field3 FROM vlfVehicleInfo WHERE VehicleId =@vehicleId;  
         IF @tmpDriverName IS NOT NULL  
         BEGIN         
            SET @message = replace(@message, '[DRIVER_NAME]', CAST(@tmpDriverName AS NVARCHAR));     
            SET @errorMessage =  @errorMessage  + '10' + @message;  
         END     
         END   
         ELSE  
         BEGIN  
         IF @driverId <> 0   
         BEGIN                                             
            SELECT @driverName = FirstName + ' ' + LastName FROM vlfDriver WHERE DriverId=@driverId;             
            IF @driverName IS NOT NULL  
            BEGIN         
            SET @message = replace(@message, '[DRIVER_NAME]', CAST(@driverName AS NVARCHAR));      
            SET @errorMessage =  @errorMessage  + '11' + @message;    
            END     
         END  
         END  
                    
      END  
  
      if CHARINDEX('[LONGITUDE]',@message) > 0  
      BEGIN  
         IF @longitude <> 0  
         SET @message = replace(@message, '[LONGITUDE]',  CAST(@longitude AS NVARCHAR));        
         SET @errorMessage =  @errorMessage  + '12' + @message;  
      END  
  
      if CHARINDEX('[LATITUDE]',@message) > 0  
      BEGIN  
      IF @latitude <> 0   
         SET @message = replace(@message, '[LATITUDE]', CAST(@latitude AS NVARCHAR));   
         SET @errorMessage =  @errorMessage  + '13' + @message;  
      END  
  
      if CHARINDEX('[ADDRESS]',@message) > 0  
      BEGIN  
      IF @address IS NOT NULL               
         SET @message = replace(@message, '[ADDRESS]', @address);           
         SET @errorMessage =  @errorMessage  + '14' + @message;     
      END  
  
      if CHARINDEX('[StDate]',@message) > 0  
      BEGIN  
      IF @date IS NOT NULL  
         SET @message = replace(@message, '[StDate]', CAST(@date AS NVARCHAR) + '(GMT' + CONVERT(nvarchar, @timeZone) + ')');  
         SET @errorMessage =  @errorMessage  + '15' + @message;  
      END  
  
      if CHARINDEX('[EVENT_TIME]',@message) > 0  
      BEGIN  
         IF @eventTime IS NOT NULL       
         SET @message = replace(@message, '[EVENT_TIME]', CAST(@eventTime AS nvarchar) + '(GMT' + CONVERT(nvarchar, @timeZone) + ')');  
         ELSE  
         SET @message = replace(@message, '[EVENT_TIME]', CAST(GETDATE() AS NVARCHAR));  
  
       SET @errorMessage =  @errorMessage  + '16' + @message;  
      END  
  
      if CHARINDEX('[LANDMARK_NAME]',@message) > 0  
      BEGIN  
         IF @landmarkName IS NOT NULL  
         SET @message = replace(@message, '[LANDMARK_NAME]', @landmarkName);  
       SET @errorMessage =  @errorMessage  + '17' + @message;  
      END  
                
      if CHARINDEX('[LANDMARK_ID]',@message) > 0     
      BEGIN           
       IF @landmarkid <> 0  
       BEGIN  
          SET @message = replace(@message, '[LANDMARK_ID]', CAST(@landmarkid AS NVARCHAR));      
          SET @errorMessage =  @errorMessage  + '18' + @message;   
       END        
      END  
  
      if CHARINDEX('[FLEET_ID]',@message) > 0  
      BEGIN                    
         IF @objects = 'Fleet' AND @objectId > 0  
         BEGIN  
         SET @message = replace(@message, '[FLEET_ID]', CAST(@objectId AS NVARCHAR));    
         SET @errorMessage =  @errorMessage  + '19' + @message;       
         END              
      END     
  
      if CHARINDEX('[GOOGLE_LINK]',@message) > 0  
      begin  
         SET @message = replace(@message, '[GOOGLE_LINK]', 'http://maps.google.com/maps?q=' + CAST(@latitude AS NVARCHAR) + ',' + CAST(@longitude AS NVARCHAR));  
         SET @errorMessage =  @errorMessage  + '20' + @message;  
      end  
  
      IF CHARINDEX('[SUPERVISOR]',@message) > 0  
      BEGIN  
         IF @vehicleId > 0  
         BEGIN  
         SET @superName = '';  
  
         SELECT @superName = Field4 FROM vlfVehicleInfo WHERE VehicleId = @vehicleId;  
         IF @superName <> '' AND @superName IS NOT NULL  
         BEGIN  
            SET @message = replace(@message, '[SUPERVISOR]', @superName);  
            SET @errorMessage =  @errorMessage  + '21'+  @message;  
         END  
         END  
      END  
  
      IF CHARINDEX('[LICENSE_PLATE]',@message) > 0  
      BEGIN  
         IF @vehicleId > 0  
         BEGIN  
         SET @lp = '';  
  
         SELECT @lp = LicensePlate FROM vlfVehicleAssignment WHERE VehicleId = @vehicleId;  
         IF @lp <> '' AND @lp IS NOT NULL  
         BEGIN  
            SET @message = replace(@message, '[LICENSE_PLATE]', @lp);  
            SET @errorMessage =  @errorMessage  + '22' + @message;  
         END  
         END  
      END  
  
      if CHARINDEX('[DISPUTE_LINK]',@message) > 0  
      begin  
      
        IF @speed  > 0  
        BEGIN  
        SET  @speedUnit =  CAST(@speed AS NVARCHAR) + ' ' + @metricUnit;              
        END  
  
        IF @roadSpeed  > 0  
        BEGIN  
        SET  @tmpDisputeRoadSpeed =  CAST(@roadSpeed AS NVARCHAR) + ' ' + @metricUnit;                          
       END  
                  
       SET @message = replace(@message, '[DISPUTE_LINK]', 'http://preprod.sentinelfm.com/community/dispute.aspx?infractionid=' + CAST(@violationId AS NVARCHAR) + '&q=' + CAST(@latitude AS NVARCHAR) + ',' + CAST(@longitude AS NVARCHAR) + '&errorspeed=' + 
@tmpDisputeRoadSpeed + '&yourspeed=' + @speedUnit + '&address=' + @address + '&objects=' + @objects + '&objectid=' + CAST(@objectId AS nvarchar) + '&nid=[NOTIFICATION_ID]');  
       SET @errorMessage =  @errorMessage  + '23'+ @message;  
      end  
  
      if CHARINDEX('[CLT_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CLT_VALUE]', @cltValue);  
       SET @errorMessage =  @errorMessage  + '24'+ @message;  
      END  
  
      if CHARINDEX('[EOP_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[EOP_VALUE]', @eopValue);  
       SET @errorMessage =  @errorMessage  + '25'+ @message;  
      END  
  
      if CHARINDEX('[EOT_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[EOT_VALUE]', @eotValue);  
       SET @errorMessage =  @errorMessage  + '26'+ @message;  
      END  
  
      if CHARINDEX('[STA_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[STA_VALUE]', @staValue);  
       SET @errorMessage =  @errorMessage  + '27'+ @message;  
      END  
  
      if CHARINDEX('[FLIP_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[FLIP_VALUE]', @flipValue);  
       SET @errorMessage =  @errorMessage  + '28'+ @message;  
      END  
  
      if CHARINDEX('[FLIS_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[FLIS_VALUE]', @flisValue);  
       SET @errorMessage =  @errorMessage  + '29'+ @message;  
      END  
  
      if CHARINDEX('[RPM_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[RPM_VALUE]', @rpmValue);  
       SET @errorMessage =  @errorMessage  + '30'+ @message;  
      END  
  
      if CHARINDEX('[TVD_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[TVD_VALUE]', @tvdValue);  
       SET @errorMessage =  @errorMessage  + '31'+ @message;  
      END  
  
      if CHARINDEX('[DTC_VALUE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[DTC_VALUE]', @dtcValue);  
       SET @errorMessage = @errorMessage + '32'+  @message;  
      END  
  
      if CHARINDEX('[VEHICLE_YEAR]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_YEAR]', @modelYear);  
       SET @errorMessage =  @errorMessage  + '33'+  @message;  
      END  
  
      if CHARINDEX('[VEHICLE_MODEL]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_MODEL]', @model);  
       SET @errorMessage =  @errorMessage  + '34'+  @message;  
      END     
  
      if CHARINDEX('[VEHICLE_MAKE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_MAKE]', @make);  
       SET @errorMessage =  @errorMessage  + '35'+ @message;  
      END  
  
      if CHARINDEX('[VEHICLE_VIN]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[VEHICLE_VIN]', @vin);  
       SET @errorMessage =  @errorMessage  + '36'+ @message;  
      END  
  
      if CHARINDEX('[CUSTOM_FIELD1]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CUSTOM_FIELD1]', @customField1);  
       SET @errorMessage =  @errorMessage  + '37'+ @message;  
      END  
  
      if CHARINDEX('[CUSTOM_FIELD2]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CUSTOM_FIELD2]', @customField2);  
       SET @errorMessage =  @errorMessage  + '38'+ @message;  
      END  
  
      if CHARINDEX('[CUSTOM_FIELD3]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CUSTOM_FIELD3]', @customField3);  
       SET @errorMessage =  @errorMessage  + '39'+ @message;  
      END  
  
      if CHARINDEX('[FLEETS_NAME]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[FLEETS_NAME]', @fleetsName);  
       SET @errorMessage =  @errorMessage  + '40'+ @message;  
      END  
  
      if CHARINDEX('[LSD]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[LSD]', @lsdSetting);  
       SET @errorMessage =  @errorMessage  + '41'+ @message;  
      END  
  
      if CHARINDEX('[LSD_RULESETTINGS]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[LSD_RULESETTINGS]', @lsdSetting);  
       SET @errorMessage =  @errorMessage  + '42'+ @message;  
      END  
      if CHARINDEX('[KEYFOB_ID]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[KEYFOB_ID]', @personId);  
      END  
     
      IF CHARINDEX('[CONTACT_NAME]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CONTACT_NAME]', @LmkContactName); --??  
       SET @errorMessage = @errorMessage + '43'+  @message;  
      END  
     
      IF CHARINDEX('[CONTACT_PHONE]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[CONTACT_PHONE]', @LmkPhoneNum); --??  
       SET @errorMessage =  @errorMessage  + '44'+ @message;  
      END  
     
      IF CHARINDEX('[PROBLEM_DESCRIPTION]',@message) > 0     
      BEGIN  
       SELECT @ProblemDescription = ISNULL(Field1,'') FROM vlfVehicleExtraServiceHistory WHERE VehicleId=@vehicleId;   
       SET @message = REPLACE(@message, '[PROBLEM_DESCRIPTION]', @ProblemDescription); --??  
       SET @errorMessage =  @errorMessage  + '45'+ @message;  
      END  
      
      IF CHARINDEX('[SERVICE_CONTACT]',@message) > 0     
                 BEGIN  
                                SELECT @ServiceContact = ISNULL(Field2,'') FROM vlfVehicleExtraServiceHistory  WHERE VehicleId=@VehicleId; 
                                SET @message = REPLACE(@message, '[SERVICE_CONTACT]', @ServiceContact); --?? 
                                SET @errorMessage =  @errorMessage  + '48'+ @message;
                 END
  
      IF CHARINDEX('[MAIN_BATTERY]',@message) > 0     
      BEGIN    
       SET @message = REPLACE(@message, '[MAIN_BATTERY]', @mainbattery); --??  
       SET @errorMessage =  @errorMessage  + '46'+ @message;  
      END  
  
      IF CHARINDEX('[S4]',@message) > 0     
      BEGIN        
       SET @message = REPLACE(@message, '[S4]', dbo.GetStringValueFromCustomProperties(@customProp, 'S4'));  
       SET @errorMessage =  @errorMessage  + '47'+ @message;  
      END  
   IF CHARINDEX('[ASSIGNED_FLEET]',@message) > 0     
   BEGIN        
               IF @objects = 'Fleet' AND @objectId > 0
               BEGIN
                              select @assignedFleetName = Nodecode from  dbo.udf_VehicleCostCenters (@organizationId) where VehicleID=@vehicleId;
                              IF @assignedFleetName IS NOT NULL
                              BEGIN
                                             SET @message = REPLACE(@message, '[ASSIGNED_FLEET]', @assignedFleetName);
                              END
               END
      
   END  

   IF CHARINDEX('[FORMATTED_DATETIME]',@message) > 0     
   BEGIN        
    SET @message = REPLACE(@message, '[FORMATTED_DATETIME]', @formattedDateTime);  
   END
   
   if CHARINDEX('[VEHICLE_OPERATIONAL_STATE]',@message) > 0     
      BEGIN    
		   SET @message = REPLACE(@message, '[VEHICLE_OPERATIONAL_STATE]', @vehicleOperationalStateName);  
      END

   IF CHARINDEX('[SERVICE_DURATION]',@message) > 0     
   BEGIN          
                                                
               SET @msgDuration = dbo.GetStringValueFromCustomProperties(@customProp, 'DURATION');
               SET @serviceDuration = CAST(@msgDuration AS int);
               SET @msgDuration = CONVERT(varchar, DATEADD(ss, @serviceDuration, 0), 108);
    SET @message = REPLACE(@message, '[SERVICE_DURATION]', @msgDuration);  
   END
      END  
        
      END TRY  
   BEGIN CATCH  
    SET @errorMessage = ERROR_MESSAGE() + ', Line' +  CAST(ERROR_LINE() AS nvarchar);  
   END CATCH  
       
     IF @allemail = '' OR @allemail is null  
            BEGIN  
                  SET @allemail = 'mnancharla@bsmwireless.com'; 
                                                              SET @sendStatus = 1; 
            END  
  
            IF @subject = ''  
            BEGIN  
                  SET @subject = 'No subject for service config id ' + CAST(@serviceConfigId AS nvarchar) +', please input subject';  
            END  
  
            IF @message = '' OR @message is NULL  
            BEGIN  
                  SET @message = 'No message for service config id ' + CAST(@serviceConfigId AS nvarchar) +', please input message. Error message: ' + @errorMessage;  
            END  
      
   DECLARE @nid int;  
   --SET @subject = 'PREPRODUCTION - ' + @subject;  
     
   IF @scheduleSend IS NOT NULL AND @scheduleSend <> ''
   BEGIN  
  SET @sendStatus = 1;  
   END  
  
      INSERT INTO dbo.evtNotifications (EventID, FleetID, OrganizationID, BoxID, [Date], EmailList, [Subject], [Message], [Status], ServiceConfigId, Vehicleid, LandmarkId, RuleValue, Scheduled)   
      VALUES (@eventId, @fleetId, @organizationId, @boxId, @date, @allemail, @subject, LEFT(@message, 3000), @sendStatus, @serviceConfigId, @vehicleId, @landmarkId, @ruleValue, @scheduleSend);  
  
    
   SET @nid = @@identity;  
  
        
  
      SELECT @nid;  
END  



